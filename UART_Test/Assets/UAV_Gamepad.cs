// GENERATED AUTOMATICALLY FROM 'Assets/UAV_Gamepad.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @UAV_Gamepad : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @UAV_Gamepad()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""UAV_Gamepad"",
    ""maps"": [
        {
            ""name"": ""uavControl"",
            ""id"": ""dadc7866-d197-4d06-bcab-1f55a38ae468"",
            ""actions"": [
                {
                    ""name"": ""PitchRollControl"",
                    ""type"": ""Value"",
                    ""id"": ""357a2a95-9c1c-4871-978d-122a2f9a6a8f"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ThrottleYawControl"",
                    ""type"": ""Value"",
                    ""id"": ""9260f3b0-cf85-4190-8727-07f8cc0aaf56"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ArmDisarm"",
                    ""type"": ""Button"",
                    ""id"": ""f2e427f0-84f6-419d-8468-0cabd7b627fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                },
                {
                    ""name"": ""CalibrateGyro"",
                    ""type"": ""Button"",
                    ""id"": ""bab5896d-b531-4e40-824a-36d15d1898e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""MultiTap(tapCount=3)""
                },
                {
                    ""name"": ""CalibrateAcc"",
                    ""type"": ""Button"",
                    ""id"": ""8389c023-a864-46fc-82f5-9c3736da6942"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""MultiTap(tapCount=3)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e6945666-a582-4fe1-9e62-f700bb3b52ee"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PitchRollControl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a505c963-5c57-40b9-9ab5-d7d0b6a730bc"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrottleYawControl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f2c78d1-919d-4fd2-8f6a-8059b628f57a"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ArmDisarm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8aa40e1b-fd08-43f4-a87c-5958d085f3e7"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CalibrateGyro"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f987e663-9ede-4703-9d21-db48a73880a4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CalibrateAcc"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // uavControl
        m_uavControl = asset.FindActionMap("uavControl", throwIfNotFound: true);
        m_uavControl_PitchRollControl = m_uavControl.FindAction("PitchRollControl", throwIfNotFound: true);
        m_uavControl_ThrottleYawControl = m_uavControl.FindAction("ThrottleYawControl", throwIfNotFound: true);
        m_uavControl_ArmDisarm = m_uavControl.FindAction("ArmDisarm", throwIfNotFound: true);
        m_uavControl_CalibrateGyro = m_uavControl.FindAction("CalibrateGyro", throwIfNotFound: true);
        m_uavControl_CalibrateAcc = m_uavControl.FindAction("CalibrateAcc", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // uavControl
    private readonly InputActionMap m_uavControl;
    private IUavControlActions m_UavControlActionsCallbackInterface;
    private readonly InputAction m_uavControl_PitchRollControl;
    private readonly InputAction m_uavControl_ThrottleYawControl;
    private readonly InputAction m_uavControl_ArmDisarm;
    private readonly InputAction m_uavControl_CalibrateGyro;
    private readonly InputAction m_uavControl_CalibrateAcc;
    public struct UavControlActions
    {
        private @UAV_Gamepad m_Wrapper;
        public UavControlActions(@UAV_Gamepad wrapper) { m_Wrapper = wrapper; }
        public InputAction @PitchRollControl => m_Wrapper.m_uavControl_PitchRollControl;
        public InputAction @ThrottleYawControl => m_Wrapper.m_uavControl_ThrottleYawControl;
        public InputAction @ArmDisarm => m_Wrapper.m_uavControl_ArmDisarm;
        public InputAction @CalibrateGyro => m_Wrapper.m_uavControl_CalibrateGyro;
        public InputAction @CalibrateAcc => m_Wrapper.m_uavControl_CalibrateAcc;
        public InputActionMap Get() { return m_Wrapper.m_uavControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UavControlActions set) { return set.Get(); }
        public void SetCallbacks(IUavControlActions instance)
        {
            if (m_Wrapper.m_UavControlActionsCallbackInterface != null)
            {
                @PitchRollControl.started -= m_Wrapper.m_UavControlActionsCallbackInterface.OnPitchRollControl;
                @PitchRollControl.performed -= m_Wrapper.m_UavControlActionsCallbackInterface.OnPitchRollControl;
                @PitchRollControl.canceled -= m_Wrapper.m_UavControlActionsCallbackInterface.OnPitchRollControl;
                @ThrottleYawControl.started -= m_Wrapper.m_UavControlActionsCallbackInterface.OnThrottleYawControl;
                @ThrottleYawControl.performed -= m_Wrapper.m_UavControlActionsCallbackInterface.OnThrottleYawControl;
                @ThrottleYawControl.canceled -= m_Wrapper.m_UavControlActionsCallbackInterface.OnThrottleYawControl;
                @ArmDisarm.started -= m_Wrapper.m_UavControlActionsCallbackInterface.OnArmDisarm;
                @ArmDisarm.performed -= m_Wrapper.m_UavControlActionsCallbackInterface.OnArmDisarm;
                @ArmDisarm.canceled -= m_Wrapper.m_UavControlActionsCallbackInterface.OnArmDisarm;
                @CalibrateGyro.started -= m_Wrapper.m_UavControlActionsCallbackInterface.OnCalibrateGyro;
                @CalibrateGyro.performed -= m_Wrapper.m_UavControlActionsCallbackInterface.OnCalibrateGyro;
                @CalibrateGyro.canceled -= m_Wrapper.m_UavControlActionsCallbackInterface.OnCalibrateGyro;
                @CalibrateAcc.started -= m_Wrapper.m_UavControlActionsCallbackInterface.OnCalibrateAcc;
                @CalibrateAcc.performed -= m_Wrapper.m_UavControlActionsCallbackInterface.OnCalibrateAcc;
                @CalibrateAcc.canceled -= m_Wrapper.m_UavControlActionsCallbackInterface.OnCalibrateAcc;
            }
            m_Wrapper.m_UavControlActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PitchRollControl.started += instance.OnPitchRollControl;
                @PitchRollControl.performed += instance.OnPitchRollControl;
                @PitchRollControl.canceled += instance.OnPitchRollControl;
                @ThrottleYawControl.started += instance.OnThrottleYawControl;
                @ThrottleYawControl.performed += instance.OnThrottleYawControl;
                @ThrottleYawControl.canceled += instance.OnThrottleYawControl;
                @ArmDisarm.started += instance.OnArmDisarm;
                @ArmDisarm.performed += instance.OnArmDisarm;
                @ArmDisarm.canceled += instance.OnArmDisarm;
                @CalibrateGyro.started += instance.OnCalibrateGyro;
                @CalibrateGyro.performed += instance.OnCalibrateGyro;
                @CalibrateGyro.canceled += instance.OnCalibrateGyro;
                @CalibrateAcc.started += instance.OnCalibrateAcc;
                @CalibrateAcc.performed += instance.OnCalibrateAcc;
                @CalibrateAcc.canceled += instance.OnCalibrateAcc;
            }
        }
    }
    public UavControlActions @uavControl => new UavControlActions(this);
    public interface IUavControlActions
    {
        void OnPitchRollControl(InputAction.CallbackContext context);
        void OnThrottleYawControl(InputAction.CallbackContext context);
        void OnArmDisarm(InputAction.CallbackContext context);
        void OnCalibrateGyro(InputAction.CallbackContext context);
        void OnCalibrateAcc(InputAction.CallbackContext context);
    }
}
