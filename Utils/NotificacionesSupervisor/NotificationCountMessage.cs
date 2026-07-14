using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Utils.NotificacionesSupervisor {
    public class NotificationCountMessage : ValueChangedMessage<int> {
        public NotificationCountMessage(int value) : base(value) {
        }
    }
}