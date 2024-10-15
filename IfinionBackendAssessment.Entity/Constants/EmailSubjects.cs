using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Entity.Constants
{
    public static class EmailSubjects
    {
        public const string WelcomeMessage = "Welcome Message";
        public const string OrderPlcacementNotifcation = "Order Plcacement Notifcation";
        public const string OrderShipmentNotification = "Order Shipment Notification";
        public const string OrderDeliveryNotification = "Order Delivery Notification";
    }

    public static class EmailContent
    {
        public const string WelcomeMessage = "Thank you for your successful registration on our platform.\nWe're here to serve you better.\nRegards";
    }
}
