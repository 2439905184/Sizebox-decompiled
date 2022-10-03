Test = RegisterBehavior("Test Transform")
Test.agentType = "humanoid"
Test.targetType = "oneself"

--[[ This scrip consists in a recursive way to print the name of all the bones in the hierarchie 
    Each model has his own naming convention for bones. The console has limited space, you can
    read the output_log.txt file for the complete logs ]]

function Test:Start()
    self:PrintTransforms(self.agent.transform)
    Log("Head: " .. self.agent.bones.head.name)
end

function Test:PrintTransforms(transform)
    Log(transform.name)
    if transform.childCount > 0 then
        local lastChild = transform.childCount - 1
        for i=0,lastChild do
            self:PrintTransforms(transform.GetChild(i))
        end
    end
end