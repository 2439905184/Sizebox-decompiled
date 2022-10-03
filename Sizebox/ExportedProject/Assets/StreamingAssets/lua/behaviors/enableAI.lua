EnableAI = RegisterBehavior("Enable AI")
EnableAI.data = {
	secondary = true,
    agent = {
        type = { "humanoid" },
    },
    target = {
        type = { "oneself" }
    }
}

function EnableAI:Start()
    self.agent.ai.EnableAI() -- this will enable the ai mode to take decisions on their own
    -- there no disable because giving any command will automatically disable it
end

