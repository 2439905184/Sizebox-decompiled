Behavior = RegisterBehavior("behavior_name") -- The name to identify this behavior, to call it from other scripts

-- This is a comment, those will do nothing in the game, use them for comment about what does each line of code.
-- You can learn basic lua syntax, and also look in the other scripts in this folder for examples.

-- Behavior configuration (targets, menu options, ai settings)
Behavior.data = { 
    -- ENTITY OPTIONS --
    
    -- Who can do this behavior??
    agent = {
        type    = { list-of-agents },
        exclude = { list-of-agents-to-ignore }
    },
    
    -- Who is the target of the action?
    target = {
        type    = { list-of-targets },
        exclude = { list-of-targets-to-ignore }
    },
    
    -- a list is defined like this: {"a", "b", "c"}
    -- Possible types: "humanoid", "giantess", "micro, "player", "oneself", "none"

    -- MENU OPTIONS --
    -- How it appears in the menu?
    menuEntry = "MenuA/MenuB/Behavior",
    
    -- You want it to be hidden from the menu?
    hideMenu = false,
    
    -- SPECIAL ACTIONS --
    -- When Enabled AI is on, this action can be selected by a giantess.
    ai = true,
    
    -- This will make it work at the same time of another action without interrupt it.
    secondary = true,
    
    -- A secondary action can only be interrupted if you choose another action of the same flags (at least one flag)
    -- You can define your own flags.
    flags = { list-of-flags },
    
    -- This will make hidden behaviors appear in the behavior manager.
    -- Be cautious when you use this!
    forceAppearInManager = true,
    
    -- Describe what the behaviour does. Be sure to keep it short & sweet!
    description = "Changes the size of a Giantess.",
    
    -- Tags
    -- This allows you specify what this behaviour does, so that it can be displayed properly. These are not required, but are generally good idea to add. Use ',' to seperate tags.
    -- * NOTE: Although you can put anything you want in the tags, provided below is a list of common tags that you should use if your behaviour falls under any of them.
    --         This is to avoid tags that are written differently, but mean the same thing (e.g. "gts" and "giantess"), which are considered to be two seperate tags by the game.
    --
    --	* LIST OF COMMON TAGS: giantess, micro, sizechange, vore, feet, movement, morphs, ui, shrinking, growing, breasts, sound
    --
    tags = "giantess, sizechange";
    
    -- Script settings!
    -- 
    -- These will appear in the behavior manager.
    -- The available types (at the moment) are:
    --        string		: A string of text. Will appear as an input box.
    --					|
    --        bool		: True & false. Will appear as a checkbox.
    --					|
    --        float		: A floating point number between 0 and 1. Will appear as a slider.
    --					|
    --        array		: The index of the currently selected item. Will appear as a dropdown selection box.
    --					|
    --        keybind		: A valid keyboard key. Will appear as a button.
    --					: * NOTE: The def_value should be a Unity KeyCode. Example: "1" would be "alpha1"
    --					: * REFS: A complete list of KeyCodes can be found at https://docs.unity3d.com/ScriptReference/KeyCode.html .
    --
    -- Setting Formatting:
    --        var_name		: The variable name that the value will be assigned to. Make sure it's a valid name.
    --				: Example: Var name "MyVar" will be assigned to "Behavior.MyVar"
    --					|
    --        ui_text		: The text that will appear in the behavior manager next to this setting. Should describe what this setting does.
    --					|
    --        type		: The type of variable this setting should accept. Available types are listed above.
    --					|
    --        def_value		: The default value of this setting. MUST correspond with the setting type. For example, if your setting is a boolean,
    --				: then you should have "true" (without quotation marks) in the def_value field.
    --
    -- Example:
    --    MyBehavior.data = {
    --        ...
    --        
    --        settings = {
    --            { "enable_jump", "Enable jumping?", "bool", true },
    --            { "jump_anim", "Jumping animation", "string", "Jumping 1" },
    --            { "jump_height", "Jump height", "float", 0.62 },
    --            { "jump_sound", "Jumping sound", "array", 0, { "Jump sound 1", "Jump sound 2", "Jump sound 3" } },
    --            { "jump_key", "Jump key", "keybind", "space" }
    --        }
    --    }
    
    settings = {
        { "enable_jump", "Enable jumping?", "bool", true },
        { "jump_anim", "Jumping animation", "string", "Jumping 1" },
        { "jump_height", "Jump height", "float", 0.62 },
        { "jump_sound", "Jumping sound", "array", 0, { "Jump sound 1", "Jump sound 2", "Jump sound 3" } },
        { "jump_key", "Jump key", "keybind", "space" }
    }
}

function Behavior:Start()
    -- This will run only at the beginning of the behavior.
end

function Behavior:Update()
    -- This is called once per frame, to update your behavior. Avoid doing heavy operations every frame to avoid lag.
end

function Behavior:Exit() 
    -- This will be called once the actions is cancelled, or a new action has been selected to replace this one.
end
