using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using AH.Max;

namespace AH.Max.System.Serialization
{

	public class GameSerialization
	{
		public const int SaveSlotMax = 10;

		private static int previousSaveSlot = 1;

		///<Summary>
		/// Use this method to save data providing a Data object. We may make mulitple version of the method to save data for different reasons.
		/// Example: player data and level data could be done seperately as to not overrde it. Plus multiple save files need to be available.
		///</Summary>
		public static void SaveData(Data _data, int _slot)
		{
			string _path = GenerateSaveFilePath(ref _slot);

			BinaryFormatter _binaryFormatter = new BinaryFormatter();
			FileStream _file = File.Create(_path);

			_data._slot = _slot;
			_data.savePath = _path;

			_binaryFormatter.Serialize(_file, _data);

			_file.Close();

			previousSaveSlot = _slot;
		}

		///<Summary>
		/// This methos returns a Data object from the local save file. Do what you want with it. 
		///</Summary>
		public static Data LoadData(int _slot)
		{
			string _path = GenerateSaveFilePath(ref _slot);

			if(File.Exists(_path))
			{
				BinaryFormatter _binaryFormatter = new BinaryFormatter();
				FileStream _file = File.Open(_path, FileMode.Open);

				Data _data = (Data)_binaryFormatter.Deserialize(_file);

				_file.Close();

				previousSaveSlot = _slot;

				return _data;
			}

			return null;
		}

		///<Summary>
		/// Generates a save data path with a given save slot
		///</Summary>
		public static string GenerateSaveFilePath(ref int _slot)
		{
			if(_slot > SaveSlotMax)
			{
				_slot = SaveSlotMax;
				Debug.LogWarning("You have attempted to access a slot that is higher than the current maximum" 
				+ ". The 10th slot will be opened.");
			}
			else if(_slot < 1)
			{
				_slot = 1;
				Debug.LogWarning("You have attempted to access a slot that is lower than 1" 
				+ ". The 1st slot will be opened.");
			}
			return Application.persistentDataPath + "/SaveSlot_" + _slot;
		}
	}
}
