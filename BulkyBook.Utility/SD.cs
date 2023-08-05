using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public static class SD
    {
        // Constantes con los Roles dela aplicacion
        public const string Role_User_Individual = "Individual";
        public const string Role_User_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

		// **** Estados de la orden
		public const string StatusPending = "Pending"; // Orden creada
		public const string StatusApproved = "Approved"; // Orden aprovada
		public const string StatusInProcess = "Processing"; // Procesando orden. Estado colocado por el admin
		public const string StatusShipped = "Shipped"; // Orden enviada. En "Exito" tendria que ser el estado final
		public const string StatusCancelled = "Cancelled"; // Orden cancelada (opcional)
		public const string StatusRefunded = "Refunded"; // Orden devuelta (opcional)
		
		// **** Estados del pago
		public const string PaymentStatusPending = "Pending"; // Pago pendiente
		public const string PaymentStatusApproved = "Approved"; // Pago aprovado. Cuando se hizo el pago
		// Pago aprovado. En caso de compañia. 30 dias para hacer el pago despues dl envio de la orden
		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment"; 
		public const string PaymentStatusRejected = "Rejected"; // Pago rechazado
	}
}
