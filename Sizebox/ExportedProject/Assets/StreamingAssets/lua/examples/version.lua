-- Understanding what version of the game your script is running under
-- allows it to make sensible decisions and be more portable.

-- This example script will show you how to check what version of
-- Sizebox the script is running under and how you can use it to run
-- different code or alternativly stop your script before it errors.

function Start()
	if Game == nil then
		-- If 'Game' isn't defined you know you're running on a very
		-- old version of Sizebox. You should probably get out of here.
		log("This version is really old! Please upgrade")

		-- If you have and Update function you might need to stop
		-- behavior here. If you have an exit function you might
		-- to check there to avoid crashing blindly into modern code.
		-- See Exit() below
		return
	end

	local reqMajor = 3
	local reqMinor = 0
	local preferredMinor = 1

	-- If you're not interested in backwards compatibily branching
	-- you can ignore everything else in this file and this code snippet
	-- to do a simple 'at least' version check.
	if not Game.Version.Require(reqMajor,reqMinor) then
		-- Inform the user with a Message as they might have no idea what happened
		Game.Message("Running Sizebox v" .. Game.Version.Text .. " Script requires v" .. reqMajor .. "." .. reqMinor .. " or later", "Unsupported Version");
		return
	end

	-- Ideally you want to keep these ints in your script if you use
	-- them more than once.
	local verMajor = Game.Version.Major
	local verMinor = Game.Version.Minor

	-- You can quickly branch your script in a new direction for
	-- compalitibly with different versions or just to end it cleanly
	if verMajor == reqMajor then
		log("Can we fix it?");
		-- Like above, a minor versions changes shouldn't change as much
		-- as a major version.
		if verMinor > preferredMinor then
			Log("Yes we can!");
		else
			Log("No, it's fucked...")
		end
	else
		-- Unlike the messagebox above we're from 'Game.Version.Require' also blocking major versions > 3
		Game.Message("I'm pretending that v" .. Game.Version.Text .. " is incompatible. Please try again on v" .. reqMajor .. ".x", "Sizebox " .. reqMajor .. " only")
	end
end

--function Exit()
--{
--	if Game == nil then return end -- Avoid crashing into new code
--}
