-- By Clay Hanson, some code from the Eat and Grow script v2.0(?) by Debolte#1622

Eat = RegisterBehavior("GtsEat")
Eat.data = {
	menuEntry = "Eat",
	ai = true,
	description = "Have a Giantess eat a micro.\n\nThis behavior marks the debut of the morphing API.",
	tags = "giantess, vore, morphs",
	agent = {
		type = { "giantess" }
	},
	target = {
		type = { "micro" }
	},
	
	--[[ Script settings!
	-- 
	-- These will appear in the behavior manager.
	-- The available types (at the moment) are:
	--		string		: A string of text. Will appear as an input box.
	--		bool		: True & false. Will appear as a checkbox.
	--		float		: A floating point number between 0 and 1. Will appear as a slider.
	--		array		: The index of the currently selected item. Will appear as a dropdown.
	--
	-- Setting Formatting:
	--		var_name	: The variable name that the value will be assigned to. Make sure it's a valid name.
	--					: Example: Var name "MyVar" will be assigned to "Eat.MyVar"
	--					|
	--		ui_text		: The text that will appear in the behavior manager next to this setting. Should describe what this setting does.
	--					|
	--		type		: The type of variable this setting should accept. Available types are listed above.
	--					|
	--		def_value	: The default value of this setting. MUST correspond with the setting type. For example, if your setting is a boolean,
	--					: then you should have "true" (without quotation marks) in the def_value field.
	--
	-- Example:
	--	MyBehavior.data = {
	--		...
	--		
	--		settings = {
	--			{ "enable_jump", "Enable jumping?", "bool", true },
	--			{ "jump_anim", "Jumping animation", "string", "Jumping 1" },
	--			{ "jump_height", "Jump height", "float", 0.62 },
	--			{ "jump_sound", "Jumping sound", "array", 0, { "Jump sound 1", "Jump sound 2", "Jump sound 3" } }
	--		}
	--	}
	--	
	]]--
	
	settings = {
		{ "use_morphs", "Use facial expressions", "bool", true }
	}
}

gulps = {
	"gulp001.ogg", "gulp002.ogg", "gulp003.ogg", "gulp004.ogg", "gulp005.ogg", "gulp006.ogg", "gulp007.ogg" 
}

--[[ Expression list!
	
Formatting:
	expressionList = {
		-- (State 0 Morphs)
		{
			{ "Default", { Default Morphs (If model is not is in the list below) } },
			{ Model name 1, { Morph 1, Morph 2, Morph n... } },
			{ Model name 2, { Morph 1, Morph 2, Morph n... } },
			{ Model name n, { Morph 1, Morph 2, Morph n... } }
		}
		-- (State 1 Morphs)
		{
			{ "Default", { Default Morphs (If model is not is in the list below) } },
			{ Model name 1, { Morph 1, Morph 2, Morph n... } },
			{ Model name 2, { Morph 1, Morph 2, Morph n... } },
			{ Model name n, { Morph 1, Morph 2, Morph n... } }
		}
		-- (State n Morphs)
		{
			{ "Default", { Default Morphs (If model is not is in the list below) } },
			{ Model name 1, { Morph 1, Morph 2, Morph n... } },
			{ Model name 2, { Morph 1, Morph 2, Morph n... } },
			{ Model name n, { Morph 1, Morph 2, Morph n... } }
		}
	}
	
Example:
	expressionList = {
		-- State 0
		{
			{ "Default", { "Wa", "A" } },
			{ "UnityChan", { "BLW_SMILE1", "MTH_A" } }
		}
	}
	
Example Explanation:
	If the model name has "UnityChan" in it and the entity is set to state 0,
	then that morph list will be used. If none exist, it will use "Default"'s morph list for that state.
]]

expressionList = {
	-- When a Giantess is looking around
	{
		{ "Default", {} }
	},
	-- When a Giantess approaches a tiny ( State 0 )
	{
		{ "Default", {} },
		{ "unitycha", { "blw_ang2", "el_smile2" } },
		{ "Monika", { "surprise", "excited", "smile" } },
		{ "Yuri", { "serious", "straight", "doubt", "shocked", "Blush 4" } },
		{ "Kashima", { "Grin", "Camera Eyes Line", "E" } }
	},
	-- When a Giantess is about to eat their catch
	{
		{ "Default", { "A" } },
		{ "unitycha", { "mth_smile1", "el_smile2", "eye_def_c", "blw_smile2" } },
		{ "Monika", { "blink", "tounge open", "straight", "smile" } },
		{ "Yuri", { "serious", "doubt", "full blush 2", "blush 4", "blink", "tounge open", "straight" } },
		{ "Kashima", { "Wa" } },
		{ "Mayoma", { "A", "Grin" } }
	}
}

function Eat:Start()
	if self.target and self.target == self.agent then
		self.target = nil
	end

	self.audio_sourceH = AudioSource:new(self.agent.bones.head)
	self.audio_sourceH.spatialBlend = 1 
	self.audio_sourceH.loop = false
	
    self.stop = false
	self.chasing = false
	self.lookedAround = false
	self.state = 1
	self.oldState = 1
	self.stateValue = 0.0
	self.stateTransitionDelay = 1000.0 -- In milliseconds
	self.stateTransitionStart = 0.0
	self.crouching = false
	
	-- Get default lower leg transform
	local lastAnimation = self.agent.animation.Get()
	self.agent.animation.SetPose("idle_00")
	self.defaultLowerLegPos = self.agent.bones.leftLowerLeg.position.y - self.agent.transform.position.y
	self.agent.animation.Set(lastAnimation)
	
	self:initMorphStates()
	math.randomseed(os.time())
end

function Eat:lookForTarget()
	self.target = self.agent.findClosestMicro()
end

function Eat:initMorphStates()
	self.mStateList = {}
	
	local isName = false
	for stateIdx, stateData in pairs(expressionList) do
		for modelMorphListIdx, modelMorphList in pairs(stateData) do
			if modelMorphList[1] == "Default" or string.find(self.agent.modelName, modelMorphList[1]) ~= nil then
				self.mStateList[stateIdx] = {}
				
				-- Insert the valid morphs into the state array
				for morphIdx, morphName in pairs(modelMorphList[2]) do
					if self.agent.morphs.HasMorph(morphName) then
						table.insert(self.mStateList[stateIdx], morphName)
					end
				end
			end
		end
	end
end

function Eat:setState(st, speed)
	-- Make sure we don't accidentally stick any morphs
	if self.state ~= self.oldState and self.oldState ~= 0 then
		for morphIdx, morphName in pairs(self.mStateList[self.oldState]) do
			self.agent.morphs.SetMorphValue(morphName, 0.0)
		end
	end
	
	self.state      = st
	self.stateValue = 0.0
	self.stateTransitionDelay = speed or self.stateTransitionDelay
	self.stateTransitionStart = Time.time * 1000.0
end

function Eat:updateExpression()
	if self.state > #self.mStateList then
		Log("State is greater than the number of morph states we have cached!")
		return
	end
	
	if not self.use_morphs then
		self.stateValue = 1.0
		self.oldState   = self.state
		return
	end
	
	-- No need to update expressions at this time.
	if self.state == self.oldState and self.stateValue == 1.0 then
		return
	end
	
	-- Restore the old state's animations
	if self.state ~= self.oldState then
		if self.oldState <= #self.mStateList and self.oldState > 0 then
			for morphIdx, morphName in pairs(self.mStateList[self.oldState]) do
				self.agent.morphs.SetMorphValue(morphName, 1.0 - self.stateValue)
			end
		end
		
		if self.stateValue >= 1.0 then
			self.oldState   = self.state
			self.stateValue = 1.0
		end
	end
	
	-- Now apply the current state morph
	if self.state ~= 0 then
		for morphIdx, morphName in pairs(self.mStateList[self.state]) do
			self.agent.morphs.SetMorphValue(morphName, self.stateValue)
		end
	end
end

function Eat:Update()
	-- Update our expression if necessary.
	if self.stateValue ~= 1.0 or self.state ~= self.oldState then
		self.stateValue = ((Time.time * 1000.0) - self.stateTransitionStart) / self.stateTransitionDelay
		if self.stateValue > 1.0 then
			self.stateValue = 1.0
		end
		
		self:updateExpression()
	end
	
	-- Reset our face, and then stop (or look for another snack).
	if not self.target then
		if self.state == self.oldState and self.stateValue == 1.0 then
			if self.stop then
				self.agent.ai.StopAction()
				self.agent.ai.StopBehavior()
				return
			end
			
			self:lookForTarget()
		end
		
		return
	end
	
	-- Look around for our next snack.
	if not self.target or self.target == nil then
		self:lookForTarget()
		return
	end
	
	-- Our snack died, somehow.
	if self.target.IsDead() then
		self.target = nil
		self.agent.ai.StopAction()
		self.agent.animation.Set("Idle 4")
		
		if  self.agent.ai.IsAIEnabled() or self.agent.ai.HasQueuedBehaviors() then
			self:setState(0, 100.0)
			self.stop  = true
			return
		end
		
		self:lookForTarget()
		return
	end

	if (self.agent.animation.Get() == "Thinking 2" and not self.agent.animation.IsInTransition()) or self.target.transform.IsChildOf(self.agent.transform) then
		-- We're holding our target!
		
		-- Handle if the target gets away
		if self.target and not self.target.transform.IsChildOf(self.agent.transform) then
			self.agent.animation.SetSpeed(1)
			self:setState(1, 500.0)
			
			self.agent.LookAt(self.target)
			
			self.agent.ai.StopAction()
			self.agent.animation.Set("Idle 4")
			self.chasing = false
			return
		end
		
		-- Let's wait until we've stopped transitioning to the eating animation.
		if self.agent.animation.Get() == "Thinking 2" and self.agent.animation.IsInTransition() then
			return
		end
		
		if self.agent.animation.Get() ~= "Thinking 2" then
			-- Raise the player up to the head, which is hopefully close to the mouth
			self.agent.ai.StopAction()
			
			if self.target.isPlayer() then
				self.agent.animation.SetSpeed(0.2)
			else
				self.agent.animation.SetSpeed(1)
			end
			
			self.agent.animation.Set("Thinking 2")
			self.agent.LookAt(nil)
		else
			-- Set the 'about to eat' expression just before we get them to our mouth
			if self.agent.animation.GetProgress() > 0.03 and self.state ~= 3 and self.agent.animation.GetProgress() < 0.3 then
				self:setState(3, 500.0)
			end
			
			-- At this point, our snack is is at our mouth, and we can 'eat' them.
			if self.agent.animation.GetProgress() > 0.24 and self.agent.animation.GetProgress() < 0.3 then
				self.agent.ai.StopAction()
				self.agent.animation.SetSpeed(1)
				
				self.audio_sourceH:Stop()
				self.gulpClip = gulps[math.random(#gulps)]
				self.audio_sourceH.clip = self.gulpClip
				self.audio_sourceH:Play()
				self.audio_sourceH.minDistance = (0.01 * (self.agent.scale * 0.25))
				self.audio_sourceH.pitch = ((0.8 * (1 / math.sqrt((self.agent.scale *1000 / 125) + 1))) + 0.8)	
				self.audio_sourceH.volume = (1.7 - self.audio_sourceH.pitch)
				
				if not self.target.isPlayer() then
					self.target.Delete()
				end
				
				self:setState(1, 100.0)
				self.target = nil
				
				if self.agent.ai.IsAIEnabled() or self.agent.ai.HasQueuedBehaviors() then
					self:setState(0, 100.0)
					self.stop  = true
				end
				
				self.agent.animation.Set("Idle 4")
				self.chasing = false
				return
			end
			
			return
		end
		
		return
	else
		if self.agent.animation.Get() == "Thinking 2" then
			self.agent.ai.StopAction()
			
			-- This ones being troublesome to catch; Try looking for another target.
			self:lookForTarget()
		end
	end
	
	local crouch = false
	local closest = self.agent.scale * 0.4
	if self.target.transform.position.y <= self.agent.transform.position.y + self.defaultLowerLegPos then
		crouch = true
		closest = self.agent.scale * 0.5
	end
	
	if self.agent.DistanceTo(Vector3.New(self.target.position.x, self.agent.position.y, self.target.position.z)) > closest then
		if not self.chasing then
			self:setState(2, 1000.0)
			self.chasing = true
			self.agent.ai.StopAction()
			self.agent.LookAt(self.target)
			self.agent.animation.Set("Walk",true)
			self.agent.MoveTo(self.target)
			self.agent.Face(self.target)
		end
	else
		if self.chasing or self.crouching ~= crouch then
			self.chasing = false
			self.crouching = crouch
			
			-- Figure out whether we should crouch or not.
			if crouch then
				self.agent.animation.Set("Crouch Idle",true)
			else
				self.agent.ai.StopAction()
				self.agent.animation.Set("Idle 4",true)
			end
			
			self.agent.grab(self.target)
		end
    end
end