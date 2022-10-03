Grow = RegisterBehavior("Stop Grow")
Grow.data = {
    menuEntry = "Size/Stop",
    flags = { "grow" },
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

function Grow:Start() 
    self.agent.grow(0)

    if self.agent.dict.OnStep then
        Event.Unregister(self.agent.dict.OnStep)
    end

    if self.agent.dict.OnCrush then
        Event.Unregister(self.agent.dict.OnCrush)
    end
end