using System;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using System.Globalization;

namespace UJBHelper.DataModel.Common
{
    public class FechaTweetsSerializer : SerializerBase<DateTime>
    {

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            context.Writer.WriteString(value.ToString(CultureInfo.InvariantCulture));

        }

        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var fecha = context.Reader.ReadString();
            return ConvertirFecha(fecha);
        }

        private DateTime ConvertirFecha(string fechaFormatoTwitter)
        {
             var format = "yyyy-MM-dd";           
            var fech = fechaFormatoTwitter.Substring(0, 10);
            var formatInfo = new DateTimeFormatInfo()
            {
                ShortDatePattern = format
            };
            DateTime dt = Convert.ToDateTime(fech, formatInfo);
            return dt;
        }
    }


    public class NotificationFechaTweetsSerializer : SerializerBase<DateTime>
    {

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            context.Writer.WriteString(value.ToString(CultureInfo.InvariantCulture));
        }

        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var fecha = context.Reader.ReadString();
            return ConvertirFecha(fecha);
        }

        private DateTime ConvertirFecha(string fechaFormatoTwitter)
        {
            var format = "yyyy-MM-dd HH:mm:ss";
            var fech = fechaFormatoTwitter.Substring(0, 19);
            var formatInfo = new DateTimeFormatInfo()
            {
                ShortDatePattern = format
                //  ShortDatePattern = format
            };
            DateTime dt = Convert.ToDateTime(fech, formatInfo);
            return dt;
        }
    }
    public class CreatedUpdatedDateSerializer : SerializerBase<DateTime>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            context.Writer.WriteString(value.ToString(CultureInfo.InvariantCulture));
        }

        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var dateString = context.Reader.ReadString();
            return ConvertDate(dateString);
        }

        private DateTime ConvertDate(string dateFromDb)
        {
            //var format = "yyyy-MM-dd";
            //var fech = dateFromDb.Substring(0, 10);
            //var formatInfo = new DateTimeFormatInfo()
            //{
            //    ShortDatePattern = format
            //};
            DateTime dt = Convert.ToDateTime(dateFromDb);
            return dt;
        }
    }
}
