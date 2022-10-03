-- The less your script does in Update() loops the faster the game will
-- run. This example will show you how to use events and how to iterate
-- over a IList or IDirectory returned by a function.

-- In a real script you might want to use a class property.
-- For brevity sake local variables are used in these example functions.

function PlayerChanged()
	-- 'Game.GetLocalPlayer()' is like any other entity
	-- 'Game.GetLocalPlayerClass()' would return a player class
	local newPlayer = Game.GetLocalPlayer()

	if newPlayer != nil then
		log("User is now playing as " .. newPlayer.name .. " (" .. newPlayer.id .. ")")
	else
		log("User is not playing")
	end
end

function SelectionChanged()
	-- Game.GetLocalSelection would use the same code as player above
	-- Game.GetLocalSelections returns IList for multi-selections
	-- IList are converted to a Lua table
	local selection = Game.GetLocalSelections()

	if selection == nil then
		log("No selection")

		-- Don't forget to return early when you cannot continue
		return
	end

	local str = "Selected id(s): " -- Will concatinate to this string later
	local tmp = { } -- Will use this to get everything we need from the selection table

	for key, value in ipairs(selection) do -- Iterate over any table like this
		tmp[#tmp+1] = tostring(value.id) -- Get what we want from the selection table and place it into the tmp table as a string
	end

	str = str .. table.concat(tmp, ", ") -- Concatinate the string table to str
	log(str)
end

-- ##Order Matters!##
-- While it might feel more natural to put Start() at the top for a
-- script, other functions and variables must already be declared and
-- therefore above all calls to it in the script.

function Start()
	-- Event.Register will tell the game to automatically run your function when something happens
	-- In this example we want to know about the id of the users player and selected entities
	local Event1 = Event.Register(self, EventCode.OnLocalPlayerChanged, PlayerChanged)
	local Event2 = Event.Register(self, EventCode.OnLocalSelectionChanged, SelectionChanged)
	-- In a real scipt you could use the return value to Event.Unregister later
end

