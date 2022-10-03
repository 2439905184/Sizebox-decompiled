Gizmo = RegisterBehavior("shrink_on_step")
Gizmo.data =  {
    menuEntry = "Size/Shrink Every Step",
    secondary = true,
    agent = {
        type = { "giantess", "player" }
    },
    target = {
        type = { "oneself" }
    }
}

SHRINK_SPEED = 0.01

-- this function is not called unless you hook it to a event
-- OnStep event will pass folowing data to this function:
-- gts - giantess entity
-- position - position of the step epicenter (vector)
-- magnitude - force of the step (float)
-- foot - foot numer (0 - left, 1 - right)
function Gizmo:Listener(data)
	-- We need make sure gts.id is the same as self.agent.id otherwise all event listeners will shrink
    if data.gts.id == self.agent.id then 
        self.agent.grow(-SHRINK_SPEED, 0.3 / gts.globalSpeed)
    end
end

function Gizmo:Start()
    self.agent.dict.OnStep = Event.Register(self, EventCode.OnStep, self.Listener)
end
