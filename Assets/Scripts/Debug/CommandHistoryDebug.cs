#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

using ARKitect.Core;
using ARKitect.Commands;
using Logger = ARKitect.Core.Logger;
using System.Linq;

namespace ARKitect.Debug
{
    public class CommandHistoryDebug : MonoBehaviour
    {
        private CommandManager _commandManager;
        private CommandHistory _commandHistory;

        // Reflection: CommandHistory field infos
        private FieldInfo[] cmdHistoryFields;
        private int historyListFieldIndex = 0;
        private int currentCommandFieldIndex = 0;
        private int maxSizeFieldIndex = 0;
        private int cancelledCommandCountFieldIndex = 0;

        [Header("Data")]
        [SerializeReference]
        private ICommand[] _commands;
        private LinkedList<ICommand> _commandsOriginal;

        [SerializeReference]
        private ICommand _current;
        private LinkedListNode<ICommand> _currentOriginal;

        [SerializeField]
        private int _maxSize;
        [SerializeField]
        private int _cancelledCommandCount;

        private void Awake()
        {
            _commandManager = ARKitectApp.Instance.CommandManager;
        }

        private void Start()
        {
            _commandHistory = GetHistoryInstance();
            GetCommandHistoryFieldData();

            _commandsOriginal = (LinkedList<ICommand>)cmdHistoryFields[historyListFieldIndex].GetValue(_commandHistory);
            _currentOriginal = (LinkedListNode<ICommand>)cmdHistoryFields[currentCommandFieldIndex].GetValue(_commandHistory);
        }

        private void Update()
        {
            _commands = _commandsOriginal.ToArray();
            _current = _currentOriginal?.Value;
            _maxSize = (int)cmdHistoryFields[maxSizeFieldIndex].GetValue(_commandHistory);
            _cancelledCommandCount = (int)cmdHistoryFields[cancelledCommandCountFieldIndex].GetValue(_commandHistory);
        }

        private CommandHistory GetHistoryInstance()
        {
            FieldInfo[] fields;

            // Get the type of CommandManager
            Type type = typeof(CommandManager);
            fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            // Display the values of the fields.
            /*
            Logger.LogWarning($"Displaying the values of the fields of {type}:");
            for (int i = 0; i < fields.Length; i++)
            {
                Logger.LogWarning($"{fields[i].Name}:\t'{fields[i].GetValue(_commandManager)}'");
            }
            */

            // Find field index
            int i;
            for (i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name == "_commands") break;
            }

            return (CommandHistory)fields[i].GetValue(_commandManager);
        }

        private void GetCommandHistoryFieldData()
        {
            // Get the type of CommandHistory
            Type type = typeof(CommandHistory);
            cmdHistoryFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Display the values of the fields.
            /*
            Logger.LogWarning($"Displaying the values of the fields of {type}:");
            for (int i = 0; i < cmdHistoryFields.Length; i++)
            {
                Logger.LogWarning($"{cmdHistoryFields[i].Name}:\t'{cmdHistoryFields[i].GetValue(_commandHistory)}'");
            }
            */

            // Find field indices
            for (int i = 0; i < cmdHistoryFields.Length; i++)
            {
                if (cmdHistoryFields[i].Name == "_commands") historyListFieldIndex = i;
                if (cmdHistoryFields[i].Name == "_current") currentCommandFieldIndex = i;
                if (cmdHistoryFields[i].Name == "_maxSize") maxSizeFieldIndex = i;
                if (cmdHistoryFields[i].Name == "_cancelledCommandCount") cancelledCommandCountFieldIndex = i;
            }
        }
    }

}
#endif
