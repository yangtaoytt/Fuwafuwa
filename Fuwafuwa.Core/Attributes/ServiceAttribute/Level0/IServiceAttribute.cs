using Fuwafuwa.Core.Data.ServiceData.Level0;

namespace Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;

public interface IServiceAttribute { }

public abstract class IServiceAttribute<TServiceData> : IServiceAttribute where TServiceData : IServiceData {
    public abstract Certificate GetCertificate(TServiceData serviceData);
}

// through the Inner class and private constructor, the IServiceAttribute class can only be instantiated by the inner class.
public class Certificate {
    private Certificate(IServiceAttribute serviceAttribute, IServiceData serviceData) {
        ServiceAttribute = serviceAttribute;
        ServiceData = serviceData;
    }

    public IServiceAttribute ServiceAttribute { get; init; }
    public IServiceData ServiceData { get; init; }

    public abstract class IServiceAttribute<TServiceAttribute, TServiceData> : IServiceAttribute<TServiceData>
        where TServiceData : IServiceData
        where TServiceAttribute : IServiceAttribute<TServiceAttribute, TServiceData>, new() {
        private static readonly TServiceAttribute ServiceAttribute = new();

        public override Certificate GetCertificate(TServiceData serviceData) {
            return new Certificate(this, serviceData);
        }

        public static TServiceAttribute GetInstance() {
            return ServiceAttribute;
        }
    }
}