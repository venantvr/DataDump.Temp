using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataDumper.Interfaces;

namespace DataDumper.Repository
{
    public class BaseDumpRepository : IDisposable
    {
        private readonly IDumper _dumper;

        private readonly Dictionary<Type, Dictionary<string, Func<object>>> _registry;

        public BaseDumpRepository(IDumper dumper)
        {
            _dumper = dumper;
            _registry = new Dictionary<Type, Dictionary<string, Func<object>>>();
        }

        public Stream BaseStream => _dumper.BaseStream;

        /// <summary>
        ///     Ex�cute les t�ches d�finies par l'application associ�es � la lib�ration ou � la red�finition des ressources non
        ///     manag�es.
        /// </summary>
        public void Dispose()
        {
            Flush();
        }

        public void Add<TEntity>(string name, Func<object> entity)
        {
            var type = typeof (TEntity);

            if (!_registry.ContainsKey(type))
            {
                _registry.Add(type, new Dictionary<string, Func<object>>());
            }

            _registry[type].Add(name, entity);
        }

        //public void Dump<TEntity>(string name, Func<object> entity)
        //{
        //    _dumper?.Dump(name, entity.Invoke());
        //}

        public Dictionary<string, Func<object>> LambdaValues(Type type)
        {
            return _registry[type];
        }

        private void Flush()
        {
            foreach (var obj in _registry.Keys.SelectMany(LambdaValues))
            {
                _dumper?.Dump(obj.Key, obj.Value.Invoke());
            }
        }
    }
}