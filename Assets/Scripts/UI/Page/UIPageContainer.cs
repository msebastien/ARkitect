using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Assertions;

using ARKitect.Coroutine;
using ARKitect.UI.Modal;
using Logger = ARKitect.Core.Logger;


namespace ARKitect.UI.Page
{
    [AddComponentMenu("ARkitect/UI/Page/Page Container")]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPageContainer : Core.Singleton<UIPageContainer>
    {
        private readonly List<string> _orderedPageIds = new List<string>();

        private readonly Dictionary<string, UIPage> _pages = new Dictionary<string, UIPage>();

        private CanvasGroup _canvasGroup;

        private bool _isActivePageStacked;

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
        public IReadOnlyList<string> OrderedPagesIds => _orderedPageIds;

        /// <summary>
        ///     Map of PageId to Page.
        /// </summary>
        public IReadOnlyDictionary<string, UIPage> Pages => _pages;

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            ToggleAllPages(true); // Show/Enable all pages to init them
            RegisterPages();
        }

        private void Start()
        {
            ToggleAllPages(false); // Hide/Disable all pages
        }

        private void OnDestroy()
        {
            foreach (var pageId in _orderedPageIds)
            {
                var page = _pages[pageId];
                Destroy(page.gameObject);
            }
            _pages.Clear();
            _orderedPageIds.Clear();
        }

        private void RegisterPages()
        {
            var pages = GetComponentsInChildren<UIPage>(true);
            foreach (var page in pages)
            {
                if (!_pages.TryAdd(page.Id, page))
                {
                    Logger.LogError("This page has not been registered as it has the same Id as another page");
                }
            }
        }

        private void ToggleAllPages(bool enable)
        {
            var pages = GetComponentsInChildren<UIPage>(true);
            foreach (var page in pages)
            {
                page.gameObject.SetActive(enable);
            }
        }

        public AsyncProcessHandle Push(string pageId, bool playAnimation = true, Action<UIPage> onLoad = null)
        {
            return CoroutineManager.Instance.Run(PushRoutine(pageId, playAnimation, onLoad));
        }

        public AsyncProcessHandle Pop(bool playAnimation = true, int popCount = 1)
        {
            return CoroutineManager.Instance.Run(PopRoutine(playAnimation, popCount));
        }

        public AsyncProcessHandle Pop(bool playAnimation, string destinationPageId)
        {
            var popCount = 0;
            for (var i = _orderedPageIds.Count - 1; i >= 0; i--)
            {
                var pageId = _orderedPageIds[i];
                if (pageId == destinationPageId)
                    break;

                popCount++;
            }

            if (popCount == _orderedPageIds.Count)
                throw new Exception($"The page with id '{destinationPageId}' is not found.");

            return CoroutineManager.Instance.Run(PopRoutine(playAnimation, popCount));
        }

        private IEnumerator PushRoutine(string pageId, bool playAnimation, Action<UIPage> onLoad = null)
        {
            if (String.IsNullOrEmpty(pageId)) { Logger.LogInfo("Page Id is null or empty!"); yield break; }

            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            IsInTransition = true;

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    Interactable = false;
                    UIModalContainer.Instance.Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            // Get the enter page reference
            if (String.IsNullOrWhiteSpace(pageId)) yield break;

            if (!_pages.TryGetValue(pageId, out var enterPage))
            {
                Logger.LogError($"Unable to get the page from the register with the specified Id ({pageId})");
                yield break;
            }

            // Init
            onLoad?.Invoke(enterPage);
            enterPage.Init((RectTransform)transform);

            // Retrieve Exit page
            var exitPageId = _orderedPageIds.Count == 0 ? null : _orderedPageIds[_orderedPageIds.Count - 1];
            var exitPage = exitPageId == null ? null : _pages[exitPageId];

            // Preprocess
            exitPage?.BeforeExit(true, enterPage);
            enterPage.BeforeEnter(true, exitPage);

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>();
            if (exitPage != null)
            {
                animationHandles.Add(exitPage.Exit(enterPage, playAnimation));
                animationHandles.Add(enterPage.Enter(exitPage, playAnimation));
            }
            else
            {
                animationHandles.Add(enterPage.Enter(enterPage, playAnimation));
            }

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            _orderedPageIds.Add(pageId); // Add the pushed page to the "stack"
            IsInTransition = false;

            // Postprocess
            exitPage?.AfterExit(enterPage);
            enterPage.AfterEnter(exitPage);

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (!IsInTransition
                        && !UIModalContainer.Instance.IsInTransition)
                    {
                        Interactable = true;
                        UIModalContainer.Instance.Interactable = true;
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

            if (_pages.Count < popCount)
                throw new InvalidOperationException(
                    "Cannot transition because the page count is less than the pop count.");

            if (IsInTransition)
                throw new InvalidOperationException(
                    "Cannot transition because the screen is already in transition.");

            IsInTransition = true;

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    UIModalContainer.Instance.Interactable = false;
                    Interactable = false;
                }
                else
                {
                    Interactable = false;
                }
            }

            var exitPageId = _orderedPageIds[_orderedPageIds.Count - 1];
            var exitPage = _pages[exitPageId];

            var unusedPageIds = new List<string>();
            var unusedPages = new List<UIPage>();
            for (var i = _orderedPageIds.Count - 1; i >= _orderedPageIds.Count - popCount; i--)
            {
                var unusedPageId = _orderedPageIds[i];
                unusedPageIds.Add(unusedPageId);
                unusedPages.Add(_pages[unusedPageId]);
            }

            var enterPageIndex = _orderedPageIds.Count - popCount - 1;
            var enterPageId = enterPageIndex < 0 ? null : _orderedPageIds[enterPageIndex];
            var enterPage = enterPageId == null ? null : _pages[enterPageId];

            // Preprocess
            exitPage.BeforeExit(false, enterPage);
            enterPage?.BeforeEnter(false, exitPage);

            // Play Animations
            var animationHandles = new List<AsyncProcessHandle>();
            if (enterPage != null)
            {
                animationHandles.Add(exitPage.Exit(enterPage, playAnimation));
                animationHandles.Add(enterPage.Enter(exitPage, playAnimation));
            }
            else
            {
                animationHandles.Add(exitPage.Exit(exitPage, playAnimation));
            }

            foreach (var coroutineHandle in animationHandles)
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;

            // End Transition
            for (var i = 0; i < unusedPageIds.Count; i++)
            {
                _orderedPageIds.RemoveAt(_orderedPageIds.Count - 1);
            }
            IsInTransition = false;

            // Postprocess
            exitPage?.AfterExit(enterPage);
            enterPage?.AfterEnter(exitPage);

            // Disable Unused/Exited Pages
            for (var i = 0; i < unusedPageIds.Count; i++)
            {
                var unusedPage = unusedPages[i];
                unusedPage.gameObject.SetActive(false);
            }

            if (!_enableInteractionInTransition)
            {
                if (_enableControlInteractionsOfAllContainers)
                {
                    // If there's a container in transition, it should restore Interactive to true when the transition is finished.
                    // So, do nothing here if there's a transitioning container.
                    if (!IsInTransition
                        && !UIModalContainer.Instance.IsInTransition)
                    {
                        Interactable = true;
                        UIModalContainer.Instance.Interactable = true;
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