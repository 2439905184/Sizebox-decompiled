-- Common variables

G_AGENT = { type = { "humanoid" } }
G_TARGET = { type = { "oneself" } }
G_FLAGS = { "be" }

G_SETTINGS = {
    {"boneNames", "Bone Names", "string", "Breast,Ichichi"},
    {"limit", "Limit Size", "float", "3", {"1.2", "8"}},
    {"speed", "Rate of Change", "float", "0.03", {"0.03", "0.2"}}
}

-- Common functions

function FindBoneNames(entity, boneNames)
       local count = 0
       local foundBones = {}

       for boneName in string.gmatch(boneNames, '([^,]+)') do
               local bones = entity.bones.GetBonesByName(boneName, true)
               if bones then
                       for k,v in ipairs(bones) do
                               table.insert(foundBones, v)
                               count = count + 1
                       end
               end
       end

       if count > 0 then
           return foundBones
       else
           return nil
       end
end

-- Behaviours

BE_expand = RegisterBehavior("BE expand")
BE_expand.data = {
    menuEntry = "BE/Expand",
    flags = G_FLAGS,
    agent = G_AGENT,
    target = G_TARGET,
    settings = G_SETTINGS
}

function BE_expand:Start()
    self.bones = FindBoneNames(self.agent, self.boneNames)

    if not self.bones then
        Game.Toast.New().Print
            ("No bones found to expand in model " .. self.agent.name)
    elseif not self.initiated then
        self.initiated = true
    end
end

function BE_expand:Update()
    if self.initiated then
        local growAmount = self.speed * Time.deltaTime

        for k,v in ipairs(self.bones) do
            if v.localScale.y < self.limit then
                v.localScale = v.localScale * (1 + growAmount)
            end
        end
    end
end

BE_deflate = RegisterBehavior("BE deflate")
BE_deflate.data = {
    menuEntry = "BE/Shrink",
    flags = G_FLAGS,
    agent = G_AGENT,
    target = G_TARGET,
    settings = G_SETTINGS
}

function BE_deflate:Start()
    self.bones = FindBoneNames(self.agent, self.boneNames)
    self.limit = 1 / self.limit

    if not self.bones then
        Game.Toast.New().Print
            ("No bones found to expand in model " .. self.agent.name)
    elseif not self.initiated then
        self.initiated = true
    end
end

function BE_deflate:Update()
    if self.initiated then
        local growAmount = -self.speed * Time.deltaTime

        for k,v in ipairs(self.bones) do
            if v.localScale.y > self.limit then
                v.localScale = v.localScale * (1 + growAmount)
            end
        end
    end
end


BE_reset = RegisterBehavior("BE Reset")
BE_reset.data = {
    menuEntry = "BE/Reset",
    flags = G_FLAGS,
    agent = G_AGENT,
    target = G_TARGET,
    settings = G_SETTINGS
}

function BE_reset:Start()
    self.bones = FindBoneNames(self.agent, self.boneNames)

    for k,v in ipairs(self.bones) do
            v.localScale = Vector3.one
    end
end


BE_stop = RegisterBehavior("BE Stop")
BE_stop.data = {
    menuEntry = "BE/Stop",
    flags = G_FLAGS,
    agent = G_AGENT,
    target = G_TARGET
}

function BE_stop:Start()
    Game.Toast.New().Print("Stopping BE")
end
