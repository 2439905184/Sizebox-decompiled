local stompAnimModule = {}

-- this controls which foot would be chosen most often for crushing.
-- 0.4 means left foot is chosen 40% of time and right foot 60% of time
-- keep this between 0.1 and 0.7
local footPreference = 0.5

local randomizeWeights = false -- should animation weights be randomized every session

-- you can alter these weights to make some animations more or less likely to be chosen
-- if randomizeWeights is true, then each of these weights will be multiplied by random factors
-- the weights are then corrected to make sure average animation skew is equal to footPreference
local weights = {
    ["Acknowledging"]      = 1.0,
    ["Embar 2"]            = 1.0,
    ["Embar"]              = 1.0,
    ["Idle 2"]             = 1.0,
    ["Idle 3"]             = 1.0,
    ["Idle"]               = 1.0,
    ["Laughing"]           = 1.0,
    ["Look Down"]          = 1.0,
    ["Quick Formal Bow"]   = 1.0,
    ["Quick Informal Bow"] = 1.0,
    ["Sad Idle"]           = 1.0,
    ["Scratch Head"]       = 1.0,
}

-- these are skew values of each animation telling how often they result in chosing left foot
-- dont change these
local skews = {
    ["Quick Informal Bow"] = 0.7567567568,
    ["Idle 3"]             = 0.75,
    ["Acknowledging"]      = 0.7135135135,
    ["Quick Formal Bow"]   = 0.5297297297,
    ["Sad Idle"]           = 0.3739837398,
    ["Scratch Head"]       = 0.2978142077,
    ["Idle"]               = 0.1815718157,
    ["Embar"]              = 0.1653116531,
    ["Embar 2"]            = 0.1432432432,
    ["Idle 2"]             = 0.1304347826,
    ["Look Down"]          = 0.0923913043,
    ["Laughing"]           = 0.0921409214,
}

local weightSum = 0         -- sum of all weights
local weightSumLeft = 0     -- sum of weights of animations that are skewed to the left

local scoreSum = 0          -- score = weight * skew
local scoreSumLeft = 0

-- randomize weights and tally up sums 
for k,v in pairs(weights) do
    local weight = v
    if randomizeWeights then
        weight = v * math.random()
    end
    weights[k] = weight
    weightSum = weightSum + weight
    scoreSum = scoreSum + weight * skews[k]

    if skews[k] > 0.5 then
        weightSumLeft = weightSumLeft + weight
        scoreSumLeft = scoreSumLeft + weight * skews[k]
    end
end

-- SCARY BIT
local correction = ((scoreSum - scoreSumLeft) / footPreference - (weightSum - weightSumLeft)) 
                 / (weightSumLeft - scoreSumLeft / footPreference)

weightSum = 0 -- gotta recompute this now 

-- correct left-skewed animation weights to achieve average animation skew equal to footPreference
for k,v in pairs(weights) do
    if skews[k] > 0.5 then
       weights[k] = v * correction
    end
    
    weightSum = weightSum + weights[k]
end

function stompAnimModule.getRandomStompAnim()
    local pick = math.random() * weightSum
    for k,v in pairs(weights) do
        pick = pick - v
        if pick < 0 then
           return k
        end
    end
end

return stompAnimModule