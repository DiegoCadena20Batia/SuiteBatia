using BatiaSuite.Models.EntidadesLocal;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Data {
    public class LocalDatabaseService {
        private SQLiteAsyncConnection? _database;

        private async Task InitAsync() {
            if(_database != null) return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "BatiaLocalOffline.db3");

            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<ChecklistPendiente>();
        }

        public async Task<int> GuardarPendienteAsync(ChecklistPendiente pendiente) {
            await InitAsync();
            return await _database!.InsertAsync(pendiente);
        }

        public async Task<List<ChecklistPendiente>> ObtenerPendientesAsync() {
            await InitAsync();
            return await _database!.Table<ChecklistPendiente>().ToListAsync();
        }

        public async Task<int> BorrarPendienteAsync(int id) {
            await InitAsync();
            return await _database!.DeleteAsync<ChecklistPendiente>(id);
        }
    }
}
