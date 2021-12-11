// GENERATED AUTOMATICALLY FROM 'Assets/scripts/GamePadControl.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GamePadControl : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GamePadControl()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GamePadControl"",
    ""maps"": [
        {
            ""name"": ""ActionMap"",
            ""id"": ""bf3f11e8-27ef-44d4-9d0a-8b04771b816e"",
            ""actions"": [
                {
                    ""name"": ""ThrYaw"",
                    ""type"": ""Value"",
                    ""id"": ""955f5a20-7f7c-4395-a02d-7b58f06155e4"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PitchRoll"",
                    ""type"": ""Value"",
                    ""id"": ""de41202f-22b3-44ac-9c21-2ec2b3d01ed5"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FS_Mode"",
                    ""type"": ""Button"",
                    ""id"": ""ecc994cd-2f5d-46d4-8bbb-196c87db34bc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e646aaff-ee3c-4ac1-a964-37cc05af4e8b"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""ThrYaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""602df8c8-b117-4389-ae02-effc1ce2af8a"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""PitchRoll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""173c79f5-c69c-4665-acf0-c7889a422835"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""FS_Mode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""GamePad"",
            ""bindingGroup"": ""GamePad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // ActionMap
        m_ActionMap = asset.FindActionMap("ActionMap", throwIfNotFound: true);
        m_ActionMap_ThrYaw = m_ActionMap.FindAction("ThrYaw", throwIfNotFound: true);
        m_ActionMap_PitchRoll = m_ActionMap.FindAction("PitchRoll", throwIfNotFound: true);
        m_ActionMap_FS_Mode = m_ActionMap.FindAction("FS_Mode", throwIfNotFound: true);
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

    // ActionMap
    private readonly InputActionMap m_ActionMap;
    private IActionMapActions m_ActionMapActionsCallbackInterface;
    private readonly InputAction m_ActionMap_ThrYaw;
    private readonly InputAction m_ActionMap_PitchRoll;
    private readonly InputAction m_ActionMap_FS_Mode;
    public struct ActionMapActions
    {
        private @GamePadControl m_Wrapper;
        public ActionMapActions(@GamePadControl wrapper) { m_Wrapper = wrapper; }
        public InputAction @ThrYaw => m_Wrapper.m_ActionMap_ThrYaw;
        public InputAction @PitchRoll => m_Wrapper.m_ActionMap_PitchRoll;
        public InputAction @FS_Mode => m_Wrapper.m_ActionMap_FS_Mode;
        public InputActionMap Get() { return m_Wrapper.m_ActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IActionMapActions instance)
        {
            if (m_Wrapper.m_ActionMapActionsCallbackInterface != null)
            {
                @ThrYaw.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnThrYaw;
                @ThrYaw.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnThrYaw;
                @ThrYaw.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnThrYaw;
                @PitchRoll.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnPitchRoll;
                @PitchRoll.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnPitchRoll;
                @PitchRoll.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnPitchRoll;
                @FS_Mode.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnFS_Mode;
                @FS_Mode.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnFS_Mode;
                @FS_Mode.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnFS_Mode;
            }
            m_Wrapper.m_ActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ThrYaw.started += instance.OnThrYaw;
                @ThrYaw.performed += instance.OnThrYaw;
                @ThrYaw.canceled += instance.OnThrYaw;
                @PitchRoll.started += instance.OnPitchRoll;
                @PitchRoll.performed += instance.OnPitchRoll;
                @PitchRoll.canceled += instance.OnPitchRoll;
                @FS_Mode.started += instance.OnFS_Mode;
                @FS_Mode.performed += instance.OnFS_Mode;
                @FS_Mode.canceled += instance.OnFS_Mode;
            }
        }
    }
    public ActionMapActions @ActionMap => new ActionMapActions(this);
    private int m_GamePadSchemeIndex = -1;
    public InputControlScheme GamePadScheme
    {
        get
        {
            if (m_GamePadSchemeIndex == -1) m_GamePadSchemeIndex = asset.FindControlSchemeIndex("GamePad");
            return asset.controlSchemes[m_GamePadSchemeIndex];
        }
    }
    public interface IActionMapActions
    {
        void OnThrYaw(InputAction.CallbackContext context);
        void OnPitchRoll(InputAction.CallbackContext context);
        void OnFS_Mode(InputAction.CallbackContext context);
    }
}
