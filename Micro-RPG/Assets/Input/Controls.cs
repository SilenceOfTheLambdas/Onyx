// GENERATED AUTOMATICALLY FROM 'Assets/Input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""29cbf498-6dca-4c15-8794-984fe21096a5"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""a39a87d0-750f-4f69-be01-b79da9709a69"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""19c53936-0b31-4a07-a91f-fbe78b8363bf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SkillTree"",
                    ""type"": ""Button"",
                    ""id"": ""1011b6d9-1d1d-43fd-b494-631ed89fce51"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skill1"",
                    ""type"": ""Button"",
                    ""id"": ""0e4e3995-a2f9-4162-b82b-b4b2e33a40d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skill2"",
                    ""type"": ""Button"",
                    ""id"": ""90960b8b-fa10-4fc9-adb9-d8b49144d322"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skill3"",
                    ""type"": ""Button"",
                    ""id"": ""4b85878e-8dcd-47c3-b4fe-0c7afa2a93e1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skill4"",
                    ""type"": ""Button"",
                    ""id"": ""77983b55-4297-4eec-af04-89f80f5dee67"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Skill5"",
                    ""type"": ""Button"",
                    ""id"": ""c7003ae4-773f-4747-9f6c-3bcd1052980e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""e84d6003-7e9b-4fad-a4f9-d1cf75ee8e09"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""Normalize(min=-1,max=1)"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3e4b082e-a17f-48d0-8240-a3bd96679a5e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3bb0b23e-75b5-4ddd-bb77-e0254acf1ae1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5ef2fc65-74d7-4541-97f0-c6200c4d8e8f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""eea04605-4e36-45d7-a545-7309297ba541"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""XBOX Controller"",
                    ""id"": ""159fba90-cc6d-45af-a3c7-27f9ae9e0ea1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""Normalize(min=-1,max=1)"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c67b0d7a-5999-4863-b514-06b7e35ac8e3"",
                    ""path"": ""<XInputController>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a4beb8dd-1451-4f1b-ad62-6a1e811e17f3"",
                    ""path"": ""<XInputController>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8425455d-d485-47d8-bfc5-ed0efb3e5b05"",
                    ""path"": ""<XInputController>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c162859c-fc4a-43b6-a8d7-f32798834953"",
                    ""path"": ""<XInputController>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""0a5e4f97-c4e7-48e9-9306-2e9b044feee6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b0baa762-69d7-4f97-9af2-30e680af7204"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d83ea527-6a6f-4caa-a0ce-f159cc640840"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""4b285e8a-52da-427b-b588-f7fe633ef856"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9ffdf6d8-0f21-4989-ab16-eb209b53a99b"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d01211d5-6d63-4c77-88c9-f089c7cd252b"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c89eaf44-6195-420c-9350-c8807afbca5a"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40f220b7-2543-48d5-b21c-44df8f2fb1c9"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SkillTree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""388c8e81-213f-4e6f-8646-f3cab595e9b3"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""caefdfd0-8225-49f9-b563-53f912fcd642"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38670513-e4ea-4f2d-b0c7-39eafa6405d4"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""449490ef-c41e-4a6d-b433-ce9bf2ca206b"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Inventory = m_Player.FindAction("Inventory", throwIfNotFound: true);
        m_Player_SkillTree = m_Player.FindAction("SkillTree", throwIfNotFound: true);
        m_Player_Skill1 = m_Player.FindAction("Skill1", throwIfNotFound: true);
        m_Player_Skill2 = m_Player.FindAction("Skill2", throwIfNotFound: true);
        m_Player_Skill3 = m_Player.FindAction("Skill3", throwIfNotFound: true);
        m_Player_Skill4 = m_Player.FindAction("Skill4", throwIfNotFound: true);
        m_Player_Skill5 = m_Player.FindAction("Skill5", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Inventory;
    private readonly InputAction m_Player_SkillTree;
    private readonly InputAction m_Player_Skill1;
    private readonly InputAction m_Player_Skill2;
    private readonly InputAction m_Player_Skill3;
    private readonly InputAction m_Player_Skill4;
    private readonly InputAction m_Player_Skill5;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Inventory => m_Wrapper.m_Player_Inventory;
        public InputAction @SkillTree => m_Wrapper.m_Player_SkillTree;
        public InputAction @Skill1 => m_Wrapper.m_Player_Skill1;
        public InputAction @Skill2 => m_Wrapper.m_Player_Skill2;
        public InputAction @Skill3 => m_Wrapper.m_Player_Skill3;
        public InputAction @Skill4 => m_Wrapper.m_Player_Skill4;
        public InputAction @Skill5 => m_Wrapper.m_Player_Skill5;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Inventory.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory;
                @Inventory.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory;
                @Inventory.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInventory;
                @SkillTree.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkillTree;
                @SkillTree.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkillTree;
                @SkillTree.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkillTree;
                @Skill1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill1;
                @Skill1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill1;
                @Skill1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill1;
                @Skill2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill2;
                @Skill2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill2;
                @Skill2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill2;
                @Skill3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill3;
                @Skill3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill3;
                @Skill3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill3;
                @Skill4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill4;
                @Skill4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill4;
                @Skill4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill4;
                @Skill5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill5;
                @Skill5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill5;
                @Skill5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill5;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
                @SkillTree.started += instance.OnSkillTree;
                @SkillTree.performed += instance.OnSkillTree;
                @SkillTree.canceled += instance.OnSkillTree;
                @Skill1.started += instance.OnSkill1;
                @Skill1.performed += instance.OnSkill1;
                @Skill1.canceled += instance.OnSkill1;
                @Skill2.started += instance.OnSkill2;
                @Skill2.performed += instance.OnSkill2;
                @Skill2.canceled += instance.OnSkill2;
                @Skill3.started += instance.OnSkill3;
                @Skill3.performed += instance.OnSkill3;
                @Skill3.canceled += instance.OnSkill3;
                @Skill4.started += instance.OnSkill4;
                @Skill4.performed += instance.OnSkill4;
                @Skill4.canceled += instance.OnSkill4;
                @Skill5.started += instance.OnSkill5;
                @Skill5.performed += instance.OnSkill5;
                @Skill5.canceled += instance.OnSkill5;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
        void OnSkillTree(InputAction.CallbackContext context);
        void OnSkill1(InputAction.CallbackContext context);
        void OnSkill2(InputAction.CallbackContext context);
        void OnSkill3(InputAction.CallbackContext context);
        void OnSkill4(InputAction.CallbackContext context);
        void OnSkill5(InputAction.CallbackContext context);
    }
}
