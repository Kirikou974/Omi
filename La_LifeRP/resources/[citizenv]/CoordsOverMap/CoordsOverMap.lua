-- CLIENTSIDED --
local coordxyz = {}

coordxyz.position = {x = 0.5, y = 0.95, centered = true}
coordxyz.textSize = 0.35
coordxyz.textColour = {r = 255, g = 255, b = 255, a = 155}

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
		local playerPos = GetEntityCoords(GetPlayerPed(-1))
		local playerHeading = GetEntityHeading(GetPlayerPed(-1))
		Text("X: " .. math.ceil(playerPos.x) .." Y: " .. math.ceil(playerPos.y) .." Z: " .. math.ceil(playerPos.z) .." Heading: " .. math.ceil(playerHeading) .."")
	end
end)

function Text(text)
		
		-- SetTextColour(0, 0, 0, 255)
		--SetTextFont(4)
		--SetTextScale(0.378, 0.378)
		--SetTextWrap(0.0, 1.0)
		--SetTextCentre(false)
		--SetTextDropshadow(255, 255, 255, 255, 255)
		--SetTextEdge(1, 0, 0, 0, 205)
		--SetTextDropShadow()
		--SetTextEntry("STRING")
		--AddTextComponentString(text)
		-- DrawText(0.0175, 0.69)
		drawText(text, coordxyz.position.x, coordxyz.position.y, {
			size = coordxyz.textSize,
			colour = coordxyz.textColour,
			outline = true,
			centered = coordxyz.position.centered
		})
end

