ToastExample = RegisterBehavior("ToastExample")
ToastExample.data = {
	menuEntry = "Example/Toast Notifications",
	ai = false,
	agent = {
		type = { "humanoid" }
	},
	target = {
		type = { "none" }
	}
}

--[[
Toast notifications make two assumptions about the message you give it
- No toast message is historically important
- Only the latest message of each Toast ID is important

To put this into an example; in this script, we get the distance between
the agent and whatever the user has selected.

We don't care what the distance *WAS* a moment ago, only what it *IS* right now.
]]

function ToastExample:SelectionChanged()
	self.mySelection = Game.GetLocalSelection()
end

--[[
Start3() and Update3() will be run in favor of Start() and Update()
whenever present in a script on Sizebox 3.

The 3 correlates to the main version number of the running game, meaning
if and when Sizebox 4.0 comes out it will prefer Start4() and so on.
]]

function ToastExample:Start3()
	ToastExample:SelectionChanged()
	self.SelectionEvent = Event.Register(self, EventCode.OnLocalSelectionChanged, self.SelectionChanged)

	--[[
	Making a new toast to use later in Update3().

	Avoid calling Game.Toast.New() in a loop or update routine
	as you could very easily fill the screen with new toast messages
	if the code is flawed (or becomes flawed when someone edits the script later).
	--]]
	self.myToast = Game.Toast.New()
end

function ToastExample:Update3()
	if(self.mySelection == nil) then
		self.myToast.Print("Distance: No Selection")
		return
	end

	if(self.mySelection == self.agent) then
		self.myToast.Print("Distance: Agent is Selected")
		return
	end

	local distance = self.agent.DistanceTo(self.mySelection)
	self.myToast.Print("Distance: " .. distance)
end

function ToastExample:Exit3()
	self.myToast.Print(nil) -- Printing nil will make a Toast start to fade out immediatly instead of in 5 seconds
	Event.Unregister(self.SelectionEvent) -- Don't be naughty, tidy up!
end

function ToastExample:Start()
-- Older versions of Sizebox can't run this script but we should still do our best to inform the player
	log ("This script requires Sizebox 3 to run")
end
