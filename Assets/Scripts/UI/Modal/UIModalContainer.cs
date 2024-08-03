using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Assertions;

using ARKitect.Coroutine;
using ARKitect.UI.Page;
using Logger = ARKitect.Core.Logger;
using ARKitect.Core;


namespace ARKitect.UI.Modal
{
    [AddComponentMenu("ARkitect/UI/Modal/Modal Container")]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIModalContainer : Core.Singleton<UIModalContainer>
    {
        [SerializeField]
        private UIModalBackdrop _backdropPrefab;

        public List<UIModalBackdrop> Backdrops { get; } = new List<UIModalBackdrop>();

        private readonly Dictionary<string, UIModal> _modals = new Dictionary<string, UIModal>();

        private readonly List<string> _orderedModalIds = new List<string>();

        private CanvasGroup _canvasGroup;

        [SerializeField]
        private bool _enableInteractionInTransition;

        [SerializeField]
        private bool _enableControlInteractionsOfAllContainers;

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        /// <summary>
        ///     List of PageIds sorted in the order they are stacked. 
        /// </summary>
        public IReadOnlyList<string> OrderedModalIds => _orderedModalIds;

        /// <summary>
        ///     Map of PageId to Page.
        /// </summary>
        public IReadOnlyDictionary<string, UIModal> Modals => _modals;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            ToggleAllModals(true); // Show/Enable all modals to init them
            RegisterModals();
        }

        private void Start()
        {
            ToggleAllModals(false); // Hide/Disable all modals
        }

        private void OnDestroy()
        {
            foreach (var modalId in _orderedModalIds)
            {
                var modal = _modals[modalId];
                Destroy(modal.gameObject);
            }
            _modals.Clear();
            _orderedModalIds.Clear();
        }

        private void RegisterModals()
        {
            var modals = GetComponentsInChildren<UIModal>(true);
            foreach (var modal in modals)
            {
                if (!_modals.TryAdd(modal.Id, modal))
                {
                    Logger.LogError("This modal has not been registered as it has the same Id as another modal");
                }
            }
        }

        private void ToggleAllModals(bool enable)
        {
            var modals = GetComponentsInChildren<UIModal>(true);
            foreach (var modal in modals)
            {
                modal.gameObject.SetActive(enable);
            }
        }

        public AsyncProcessHandle Push(string modalId, bool playAnimation = true, Action<UIModal> onLoad = null)
        {
            return ARKitectApp.CoroutineManager.Run(PushRoutine(modalId, playAnimation, onLoad));
        }

        public AsyncProcessHandle Pop(bool playAnimation = true, int popCount = 1)
        {
            return ARKitectApp.CoroutineManager.Run(PopRoutine(playAnimation, popCount));
        }

        public AsyncProcessHandle Pop(bool playAnimation, string destinationModalId)
        {
            var popCount = 0;
            for (var i = _orderedModalIds.Count - 1; i >= 0; i--)
            {
                var modalId = _orderedModalIds[i];
                if (modalId == destinationModalId)
                    break;

                popCount++;
            }

            if (popCount == _orderedModalIds.Count)
                throw new Exception($"The modal with id '{destinationModalId}' is not found.");

            return ARKitectApp.CoroutineManager.Run(PopRoutine(playAnimation, popCount));
        }

        private IEnumerator PushRoutine(string modalId, bool playAnimation, Action<UIModal> onLoad = null)
        {
            if (String.IsNullOrEmpty(modalId)) { Logger.LogInfo("Modal Id is null or empty!"); yield break; }

            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            IsInTransition = true;

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    UIPageContainer.Instance.Interactable = false;
                    Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            // Setup Backdrop
            var backdrop = Instantiate(_backdropPrefab);
            backdrop.Setup((RectTransform)transform);
            Backdrops.Add(backdrop);

            // Get the enter modal reference
            if (String.IsNullOrWhiteSpace(modalId)) yield break;

            if (!_modals.TryGetValue(modalId, out var enterModal))
            {
                Logger.LogError($"Unable to get the page from the register with the specified Id ({modalId})");
                yield break;
            }

            // Initialize modal and load data for display
            onLoad?.Invoke(enterModal);
            enterModal.Init((RectTransform)transform);

            // Retrieve Exit modal
            var exitModalId = _orderedModalIds.Count == 0 ? null : _orderedModalIds[_orderedModalIds.Count - 1];
            var exitModal = exitModalId == null ? null : _modals[exitModalId];

            // Preprocess
            //foreach(var callbackReceiver in _callbackReceivers) callbackReceiver.BeforePush(enterModal, exitModal);
            exitModal?.BeforeExit(true, enterModal);
            enterModal.BeforeEnter(true, exitModal);

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>
            {
                backdrop.Enter(playAnimation)
            };

            if (exitModal != null)
            {
                animationHandles.Add(exitModal.Exit(enterModal, playAnimation));
                animationHandles.Add(enterModal.Enter(exitModal, playAnimation));
            }
            else
            {
                animationHandles.Add(enterModal.Enter(enterModal, playAnimation));
            }

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            _orderedModalIds.Add(modalId);
            IsInTransition = false;

            // Postprocess
            exitModal?.AfterExit(enterModal);
            enterModal.AfterEnter(exitModal);
            //foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.AfterPush(enterModal, exitModal);

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (!IsInTransition
                        && !UIPageContainer.Instance.IsInTransition)
                    {
                        Interactable = true;
                        UIPageContainer.Instance.Interactable = true;
                    }
                }
                else
                {
                    Interactable = true;
                }
            }
        }

        private IEnumerator PopRoutine(bool playAnimation, int popCount = 1)
        {
            Assert.IsTrue(popCount >= 1);

            if (_orderedModalIds.Count < popCount)
                throw new InvalidOperationException(
                    "Cannot transition because the modal count is less than the pop count.");

            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            IsInTransition = true;

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    UIPageContainer.Instance.Interactable = false;
                    Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            // Retrieve Exit modal
            var exitModalId = _orderedModalIds[_orderedModalIds.Count - 1];
            var exitModal = _modals[exitModalId];

            // Get unused modals
            var unusedModalIds = new List<string>();
            var unusedModals = new List<UIModal>();
            var unusedBackdrops = new List<UIModalBackdrop>();
            for (var i = _orderedModalIds.Count - 1; i >= _orderedModalIds.Count - popCount; i--)
            {
                var unusedModalId = _orderedModalIds[i];
                unusedModalIds.Add(unusedModalId);
                unusedModals.Add(_modals[unusedModalId]);
                unusedBackdrops.Add(Backdrops[i]);
            }

            // Retrieve Enter modal
            var enterModalIndex = _orderedModalIds.Count - popCount - 1;
            var enterModalId = enterModalIndex < 0 ? null : _orderedModalIds[enterModalIndex];
            var enterModal = enterModalId == null ? null : _modals[enterModalId];

            // Preprocess
            //foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.BeforePop(enterModal, exitModal);
            exitModal.BeforeExit(false, enterModal);
            enterModal?.BeforeEnter(false, exitModal);

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>();
            for (var i = unusedModalIds.Count - 1; i >= 0; i--)
            {
                var unusedModalId = unusedModalIds[i];
                var unusedModal = _modals[unusedModalId];
                var unusedBackdrop = unusedBackdrops[i];
                var partnerModalId = i == 0 ? enterModalId : unusedModalIds[i - 1];
                var partnerModal = partnerModalId == null ? null : _modals[partnerModalId];
                animationHandles.Add(unusedModal.Exit(partnerModal, playAnimation));
                animationHandles.Add(unusedBackdrop.Exit(playAnimation));
            }

            if (enterModal != null)
            {
                animationHandles.Add(enterModal.Enter(exitModal, playAnimation));
            }
            else
            {
                animationHandles.Add(exitModal.Exit(exitModal, playAnimation));
            }

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            for (var i = 0; i < unusedModalIds.Count; i++)
            {
                var unusedModalId = unusedModalIds[i];
                _orderedModalIds.RemoveAt(_orderedModalIds.Count - 1);
            }
            IsInTransition = false;

            // Postprocess
            exitModal.AfterExit(enterModal);
            enterModal?.AfterEnter(exitModal);
            //foreach (var callbackReceiver in _callbackReceivers) callbackReceiver.AfterPop(enterModal, exitModal);

            // Disable Unused/Exited Modals
            for (var i = 0; i < unusedModalIds.Count; i++)
            {
                var unusedModal = unusedModals[i];
                unusedModal.gameObject.SetActive(false);
            }

            // Remove unused Backdrops
            foreach (var unusedBackdrop in unusedBackdrops)
            {
                Backdrops.Remove(unusedBackdrop);
                Destroy(unusedBackdrop.gameObject);
            }

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (!IsInTransition
                        && !UIPageContainer.Instance.IsInTransition)
                    {
                        Interactable = true;
                        UIPageContainer.Instance.Interactable = true;
                    }
                }
                else
                {
                    Interactable = true;
                }
            }
        }

    }

}
