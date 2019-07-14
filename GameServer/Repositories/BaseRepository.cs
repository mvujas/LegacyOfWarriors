using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Database;
using Dapper.Contrib.Extensions;

namespace GameServer.Repositories
{
    public delegate void StatelessRepositoryConnectionHandler(IDbConnection connection);
    public delegate R StatefulRepositoryConnectionHandler<R>(IDbConnection connection);

    public abstract class BaseRepository<T, K> where T: class
    {
        public void Add(T entity)
        {
            StatelessConnectionRunningWrapper(connection => {
                connection.Insert(entity);
            });
        }

        public void Update(T entity)
        {
            StatelessConnectionRunningWrapper(connection => {
                connection.Update(entity);
            });
        }

        public void Delete(T entity) {
            StatelessConnectionRunningWrapper(connection => {
                connection.Delete(entity);
            });
        }

        public T GetById(K id) 
        {
            return StatefulConnectionRunningWrapper<T>(connection => {
                return connection.Get<T>(id);
            });
        }

        public IEnumerable<T> GetAll() {
            return StatefulConnectionRunningWrapper<IEnumerable<T>>(connection => {
                return connection.GetAll<T>();
            });
        }

        protected void StatelessConnectionRunningWrapper(StatelessRepositoryConnectionHandler handler) {
            using(var connection = GetConnection()) {
                connection.Open();
                handler(connection);
            }
        }

        protected R StatefulConnectionRunningWrapper<R>(StatefulRepositoryConnectionHandler<R> handler) {
            using(var connection = GetConnection()) {
                connection.Open();
                return handler(connection);
            }
        }

        protected IDbConnection GetConnection()
        {
            return ConnectionFactory.GetConnection();
        }
    }
}
