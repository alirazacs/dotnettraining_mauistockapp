﻿using SQLite;

namespace DotnetTrainingStockApp
{
    class DataBaseService
    {
        private const string DatabaseName = "DotnetStockApp.db3";
        private SQLiteAsyncConnection databaseConnection;
        async Task Init()
        {
            if(databaseConnection is  null)
            {
                //databaseConnection = new SQLiteAsyncConnection(DatabaseName, SQLiteOpenFlags.Create);
                databaseConnection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DatabaseName));
                await databaseConnection.CreateTableAsync<ScannedEntity>();
            }     
        }

        public async Task<List<ScannedEntity>> GetAllScannedEntities()
        {
            await Init();
            return await databaseConnection.Table<ScannedEntity>().ToListAsync();
        }

        public async Task AddScannedEntity(ScannedEntity scannedEntity)
        {
            await Init();
            await databaseConnection.InsertAsync(scannedEntity);
        }
    }

    [Table("ScannedEntity")]
    public class ScannedEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("Id")]
        public long Id { get; set; }
        [Column("ExpiryDate")]
        public string ExpiryDate { get; set; }
        [Column("Tags")]
        public string Tags { get; set; }
        [Column("Image")]
        public byte[] Image { get; set; }
    }
}
