using System;
using System.Collections.Generic;
using hwwebapi.Core;

namespace hwwebapi.Values {

    // Not Thread Safe
    public class ValuesRepository : IRepository<int, string> {

        private readonly IDictionary<int, string> values;
        private int nextId;

        public ValuesRepository() {
            this.values = new Dictionary<int, string>();
            this.nextId = 1;
        }

        public IEnumerable<string> GetAll() {
            return this.values.Values;
        }

        public bool TryGet(int id, out string value) {
            return this.values.TryGetValue(id, out value);
        }

        public int Create(string value) {
            if (value == null) {
                throw new ArgumentNullException("value");
            } else if (String.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("value", "'value' is empty or whitespace.");
            }

            var assignedId = this.nextId++;
            this.values[assignedId] = value;

            return assignedId;
        }

        public bool TryUpdate(int id, string value) {
            if (!this.values.ContainsKey(id)) {
                return false;
            }

            this.values[id] = value;

            return true;
        }

        public bool Delete(int id) {
            return this.values.Remove(id);
        }

    }

}
