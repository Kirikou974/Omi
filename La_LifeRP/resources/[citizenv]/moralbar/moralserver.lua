require "resources/mysql-async/lib/MySQL"
require "resources/mysql-async/lib/function"

RegisterServerEvent("moralbar:saveMoral")
RegisterServerEvent("moralbar:getMoral")
RegisterServerEvent("moralbar:addMoral")
RegisterServerEvent("moralbar:payMoral")

AddEventHandler("moralbar:saveMoral", function(message)
	TriggerEvent('es:getPlayerFromId', source, function(user)
		if (user) then
			MySQL.Async.fetchAll('select * from user_moral where identifier=@id', {['@id']= user.identifier}, function(result)
				if(result[1]) then
					MySQL.Async.execute('UPDATE `user_moral` SET `moralLevel`=@moralLevel WHERE `identifier`=@id', {['@id'] = user:identifier,['@moralLevel'] = message})
				else
					MySQL.Async.execute('INSERT INTO `user_moral`(`identifier`, `moralLevel`) VALUES (@id,@moralLevel)', {['@id'] = user:identifier,['@moralLevel'] = '100'})
				end	
			end)
		else
			TriggerEvent("es:desyncMsg")
		end
	end)
end)

AddEventHandler("moralbar:getMoral", function()
	print("GetMoral")
	print(source);
	TriggerEvent('es:getPlayerFromId', source, function(user)
		if (user) then
			MySQL.Async.fetchAll('select * from user_moral where identifier=@id', {['@id']= user.identifier}, function(result)
				if(result[1]) then
					print(result[1].moralLevel)
					TriggerClientEvent("moralbar:moralResult", source, result[1].moralLevel)
				else
					print("Player moral not found. Returning 100.")
					TriggerClientEvent("moralbar:moralResult", source, 100)
				end	
			end)
		else
			TriggerEvent("es:desyncMsg")
		end
	end)
end)

AddEventHandler("moralbar:payMoral", function(price, ticks)
	print("payMoral")
	TriggerEvent('es:getPlayerFromId', source, function(user)
		if (user) then
			user:removeMoney(price*ticks)
		else
			TriggerEvent("es:desyncMsg")
		end
	end)
end)
