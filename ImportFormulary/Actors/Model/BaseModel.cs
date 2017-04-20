using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImportFormulary.Actors.Models
{
    public abstract class BaseModel
    {
        public string DrugId { get; set; }
        [BsonId]
        public ObjectId Key { get; set; }
        public int DataRowFieldCount { get; set; }



        protected int? GetEmptyInt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            int i;
            if (int.TryParse(value, out i))
                return i;

            throw new NotImplementedException();
        }


        protected string GetEmptyChar(string p, int index)
        {
            if (!string.IsNullOrEmpty(p) && p.Length > index)
                return p.Substring(0, index + 1);
            else
                return null;
        }


        protected double? GetEmptyDouble(string p)
        {
            return GetDouble(p, null);
        }


        protected double? GetDouble(string p, double? defaultValue)
        {
            double returnvalue;
            if (double.TryParse(p, out returnvalue))
                return returnvalue;
            else
                return defaultValue;
        }


        protected sbyte ConvertToFormularyStatus(string lineValue)
        {
            sbyte fs;
            if (!sbyte.TryParse(lineValue, out fs))
                fs = -1;
            return fs;
        }


        protected string GetNullableValue(string p)
        {
            return string.IsNullOrWhiteSpace(p) ? null : p;
        }



        protected DateTime? GetDateTime(string p)
        {
            DateTime dt;
            if (DateTime.TryParse(p, out dt))
                return dt;
            return null;
        }
    }
}
