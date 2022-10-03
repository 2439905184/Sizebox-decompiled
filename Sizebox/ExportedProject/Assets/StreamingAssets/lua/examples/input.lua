--[[ This is an example for the use of input keys.
You can use it to trigger certain behaviors 
or to add new shortcuts that change gameplay

Check the Input documentation to find more input functions for things 
like scrollwheel, mouse position, and predefined player buttons,
axis and buttons refer to the controls set by the player in the 
input screen (the first that appear when you open the game) 
while keys are the ones that you define by script ]]--

function MousePress()
	-- 0 = Left Click; 1 = Right Click; 2 = Middle Click
    if Input.GetMouseButtonDown(2) then
        Log("Middle Click!")
    end
end

function KeyPress()
	-- EventCode.KeyDown will be sent when the key is held down 
	-- It's similar behaviour to holding down a key in text field
	-- If you just want the first press use 'Input.Get(Key/Button)Down'
	
	-- 'Input.GetKey' within a 'EventCode.KeyDown' be true while held
	-- In a similar way to holding down a key in a text field in your OS
    if Input.GetKey("backspace") then
        Log("You are currently holding the backspace key.")
    end

	-- Button is for already defined keys 
	-- Find their names in the Input tab before starting the game
	if Input.GetButtonDown("Sprint") then
        Log("Gotta Go Fast")
    end
end

function KeyRelease() -- If unsure try 'EventCode.KeyUp' with 'Input.GetKeyUp' in your script first. It will wait until the key has reset before running the function
	if Input.GetKeyUp("f12") then -- this only appear the moment you press the key, the it becomes false again, even if you keep pressing it
        Log("Welcome to the console. This message was sent by the input.lua script.")
    end
end

-- You want to avoid using Update() where ever possible
-- The less scripts with a Update() function the less work for the CPU
-- In most scripts listening to EventCode.KeyUp will be enough

function Start()
	-- The function name can technically be whatever you like
	-- For clarity sake it's (Key/Mouse)(Release/Press) here
	Event.Register(self, EventCode.KeyDown, KeyPress)
	Event.Register(self, EventCode.KeyUp, KeyRelease)
	Event.Register(self, EventCode.MouseDown, MousePress)
end

--[[ LIST OF KEYS

 "backspace",
 "delete",
 "tab",
 "clear",
 "return",
 "pause",
 "escape",
 "space",
 "up",
 "down",
 "right",
 "left",
 "insert",
 "home",
 "end",
 "page up",
 "page down",
 "f1",
 "f2",
 "f3",
 "f4",
 "f5",
 "f6",
 "f7",
 "f8",
 "f9",
 "f10",
 "f11",
 "f12",
 "f13",
 "f14",
 "f15",
 "0",
 "1",
 "2",
 "3",
 "4",
 "5",
 "6",
 "7",
 "8",
 "9",
 "!",
 "\"",
 "#",
 "$",
 "&",
 "'",
 "(",
 ")",
 "*",
 "+",
 ",",
 "-",
 ".",
 "/",
 ":",
 ";",
 "<",
 "=",
 ">",
 "?",
 "@",
 "[",
 "\\",
 "]",
 "^",
 "_",
 "`",
 "a",
 "b",
 "c",
 "d",
 "e",
 "f",
 "g",
 "h",
 "i",
 "j",
 "k",
 "l",
 "m",
 "n",
 "o",
 "p",
 "q",
 "r",
 "s",
 "t",
 "u",
 "v",
 "w",
 "x",
 "y",
 "z",
 "numlock",
 "caps lock",
 "scroll lock",
 "right shift",
 "left shift",
 "right ctrl",
 "left ctrl",
 "right alt",
 "left alt"

 ]]--
