using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTZPadController.BusinessLayer
{
    enum ZoomState
    {
        ZoomTeleSlow,
        ZoomTeleMedium,
        ZoomTeleFast,
        ZoomWideSlow,
        ZoomWideMedium,
        ZoomWideFast,
        Inactive
    }

    public enum ZoomCommand
    {
        TeleMedium,
        TeleSlow,
        TeleFast,
        WideMedium,
        WideSlow,
        WideFast,
        Stop
    }
    class ZoomStateMachine
    {
        class ZoomStateTransition
        {
            readonly ZoomState CurrentState;
            readonly ZoomCommand Command;

            public ZoomStateTransition(ZoomState currentState, ZoomCommand command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                ZoomStateTransition other = obj as ZoomStateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }
        }

        Dictionary<ZoomStateTransition, ZoomState> transitions;
        public ZoomState CurrentState { get; private set; }

        public ZoomStateMachine()
        {
            CurrentState = ZoomState.Inactive;
            transitions = new Dictionary<ZoomStateTransition, ZoomState>
            {
                { new ZoomStateTransition(ZoomState.Inactive, ZoomCommand.TeleMedium), ZoomState.ZoomTeleSlow },
                { new ZoomStateTransition(ZoomState.Inactive, ZoomCommand.TeleFast), ZoomState.ZoomTeleFast },
                { new ZoomStateTransition(ZoomState.Inactive, ZoomCommand.WideMedium), ZoomState.ZoomWideSlow },
                { new ZoomStateTransition(ZoomState.Inactive, ZoomCommand.WideFast), ZoomState.ZoomWideFast },
                
                { new ZoomStateTransition(ZoomState.ZoomTeleSlow, ZoomCommand.TeleSlow), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomTeleSlow, ZoomCommand.TeleMedium), ZoomState.ZoomTeleMedium },
                { new ZoomStateTransition(ZoomState.ZoomTeleSlow, ZoomCommand.TeleFast), ZoomState.ZoomTeleFast },
                { new ZoomStateTransition(ZoomState.ZoomTeleSlow, ZoomCommand.Stop), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomTeleSlow, ZoomCommand.WideMedium), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomTeleSlow, ZoomCommand.WideFast), ZoomState.Inactive },
                
                { new ZoomStateTransition(ZoomState.ZoomTeleMedium, ZoomCommand.TeleSlow), ZoomState.ZoomTeleSlow },
                { new ZoomStateTransition(ZoomState.ZoomTeleMedium, ZoomCommand.TeleMedium), ZoomState.ZoomTeleFast },
                { new ZoomStateTransition(ZoomState.ZoomTeleMedium, ZoomCommand.TeleFast), ZoomState.ZoomTeleFast },
                { new ZoomStateTransition(ZoomState.ZoomTeleMedium, ZoomCommand.Stop), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomTeleMedium, ZoomCommand.WideMedium), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomTeleMedium, ZoomCommand.WideFast), ZoomState.Inactive },

                { new ZoomStateTransition(ZoomState.ZoomTeleFast, ZoomCommand.TeleSlow), ZoomState.ZoomTeleMedium },
                { new ZoomStateTransition(ZoomState.ZoomTeleFast, ZoomCommand.Stop), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomTeleFast, ZoomCommand.WideMedium), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomTeleFast, ZoomCommand.WideFast), ZoomState.Inactive },

                { new ZoomStateTransition(ZoomState.ZoomWideSlow, ZoomCommand.WideMedium), ZoomState.ZoomWideMedium },
                { new ZoomStateTransition(ZoomState.ZoomWideSlow, ZoomCommand.WideFast), ZoomState.ZoomWideFast },
                { new ZoomStateTransition(ZoomState.ZoomWideSlow, ZoomCommand.WideSlow), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomWideSlow, ZoomCommand.Stop), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomWideSlow, ZoomCommand.TeleMedium), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomWideSlow, ZoomCommand.TeleFast), ZoomState.Inactive },

                { new ZoomStateTransition(ZoomState.ZoomWideMedium, ZoomCommand.WideMedium), ZoomState.ZoomWideFast },
                { new ZoomStateTransition(ZoomState.ZoomWideMedium, ZoomCommand.WideFast), ZoomState.ZoomWideFast },
                { new ZoomStateTransition(ZoomState.ZoomWideMedium, ZoomCommand.WideSlow), ZoomState.ZoomWideSlow },
                { new ZoomStateTransition(ZoomState.ZoomWideMedium, ZoomCommand.Stop), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomWideMedium, ZoomCommand.TeleMedium), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomWideMedium, ZoomCommand.TeleFast), ZoomState.Inactive },

                { new ZoomStateTransition(ZoomState.ZoomWideFast, ZoomCommand.WideSlow), ZoomState.ZoomWideMedium },
                { new ZoomStateTransition(ZoomState.ZoomWideFast, ZoomCommand.Stop), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomWideFast, ZoomCommand.TeleMedium), ZoomState.Inactive },
                { new ZoomStateTransition(ZoomState.ZoomWideFast, ZoomCommand.TeleFast), ZoomState.Inactive },
            };
        }

        public ZoomState GetNext(ZoomCommand command)
        {
            ZoomStateTransition transition = new ZoomStateTransition(CurrentState, command);
            ZoomState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
            {
                PTZLogger.Log.Error("Invalid transition: " + CurrentState + " -> " + command);
                return CurrentState;
            }
            PTZLogger.Log.Debug("Transition: " + CurrentState + " -> " + command + " Next : "+ nextState);
            return nextState;
        }

        public ZoomState MoveNext(ZoomCommand command)
        {
            CurrentState = GetNext(command);
            return CurrentState;
        }
    }
}
