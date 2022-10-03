Spurt = RegisterBehavior("Shink (Spurt)")
Spurt.data = {
    menuEntry = "Size/Shrink (Spurt)",
    flags = { "grow" },
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}

GROW_DURATION = 1
GROW_SPEED = -0.2
IDLE_DURATION = 5

function Spurt:Start()
    self.growStart = Time.time
    self.growing = true

end

function Spurt:Update()
    if self.growing then
        self.agent.scale = self.agent.scale * (1 + GROW_SPEED * Time.deltaTime)

        if Time.time >= self.growStart + GROW_DURATION then
            self.growing = false
            self.growStart = Time.time + IDLE_DURATION
        end
    else

        if Time.time >= self.growStart then
            self.growing = true
        end

    end
end