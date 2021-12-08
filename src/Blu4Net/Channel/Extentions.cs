using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    public static class Extentions
    {
        public static T Deserialize<T>(this XDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            using (var reader = document.Root.CreateReader())
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        public static IObservable<T> Retry<T>(this IObservable<T> src, TimeSpan delay)
        {
            if (delay == TimeSpan.Zero) return src.Retry();
            return src.Catch(src.DelaySubscription(delay).Retry());
        }
    }
}
