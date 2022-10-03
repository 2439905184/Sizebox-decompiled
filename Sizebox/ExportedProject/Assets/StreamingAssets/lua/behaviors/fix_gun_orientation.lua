Behavior = RegisterBehavior("GunAimFix")
Behavior.data = {
    secondary = true,
    menuEntry = "Debug/Fix Gun Orientation",
    agent = {
        type = { "micro" },
        exclude = { "player" }
    },
    target = {
        type = { "oneself" }
    }
}

function Behavior:Start()
	if self.agent.shooting.isAiming then
		self.agent.shooting.FixGunAimingOrientation()
	end
end