using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce
{
    public class DomainSupplier
    {
        IEventPublisher _eventPublisher = new EventPublisher();


        public DomainSupplier()
        {
        }


        public DomainSupplier(Guid id, string addressName, string email, string phone
            , string address, double addressLatitude, double addressLongitude, string note)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = new Supplier();
                temp.Id = id;
                temp.CreatedDate = DateTime.Now;
                temp.Address = address;
                temp.Phone = phone;
                temp.Email = email;
                temp.AddressName = addressName;
                temp.AddressLongitude = addressLongitude;
                temp.Note = note;
                temp.AddressLatitude = addressLatitude;

                db.Suppliers.Add(temp);
                db.SaveChanges();
            }
        }


        public void Update(Guid id, string addressName, string email, string phone
            , string address, double addressLatitude, double addressLongitude, string note)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Suppliers.SingleOrDefault(i => i.Id == id);

                temp.Id = id;
                temp.CreatedDate = DateTime.Now;
                temp.Address = address;
                temp.Phone = phone;
                temp.Email = email;
                temp.AddressName = addressName;
                temp.AddressLongitude = addressLongitude;
                temp.Note = note;
                temp.AddressLatitude = addressLatitude;

                db.SaveChanges();
            }
        }

        public void SaveSuppliersToProduct(Guid productId, List<Guid> supplierIds)
        {
            supplierIds = supplierIds.Distinct().ToList();

            _eventPublisher.Publish(new RelationShipFromRemoved(productId,"Product"));
            _eventPublisher.Publish(new RelationShipAddedOneFromWithManyTo(productId, supplierIds, "Product", "Supplier"));
        }

        public void Delete(Guid id)
        {
            using (var db = new CoreEcommerceDbContext())
            {
                var temp = db.Suppliers.SingleOrDefault(i => i.Id == id);
                db.Suppliers.Remove(temp);

                db.SaveChanges();
            }
        }
    }
}
