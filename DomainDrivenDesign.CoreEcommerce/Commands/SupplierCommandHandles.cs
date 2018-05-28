using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Commands;
using DomainDrivenDesign.Core.Implements.Events;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
   public  class SupplierCommandHandles:ICommandHandle<CreateSupplier>, ICommandHandle<UpdateSupplier>
        ,ICommandHandle<DeleteSupplier>,ICommandHandle<AddSuppliersToProduct>
    {
         public void Handle(CreateSupplier c)
        {
            new DomainSupplier(c.Id,c.AddressName,c.Email,c.Phone,c.Address,c.AddressLatitude,c.AddressLongitude,c.Note);
        }

        public void Handle(UpdateSupplier c)
        {
            new DomainSupplier().Update(c.Id, c.AddressName, c.Email, c.Phone, c.Address, c.AddressLatitude, c.AddressLongitude, c.Note);
        }

        public void Handle(DeleteSupplier c)
        {
           new DomainSupplier().Delete(c.Id);
        }

        public void Handle(AddSuppliersToProduct c)
        {
            new DomainSupplier().SaveSuppliersToProduct(c.ProductId, c.SupplierIds);
        }
    }

    public class CreateSupplier:AdminBaseCommand
    {
        public Guid Id { get; }
        public string AddressName { get; }
        public string Email { get; }
        public string Phone { get; }
        public string Address { get; }
        public double AddressLatitude { get; }
        public double AddressLongitude { get; }
        public string Note { get; }

        public CreateSupplier(Guid id, string addressName, string email, string phone
            , string address, double addressLatitude, double addressLongitude, string note,
            Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            AddressName = addressName;
            Email = email;
            Phone = phone;
            Address = address;
            AddressLatitude = addressLatitude;
            AddressLongitude = addressLongitude;
            Note = note;
        }

    }

    public class UpdateSupplier : AdminBaseCommand
    {
        public Guid Id { get; }
        public string AddressName { get; }
        public string Email { get; }
        public string Phone { get; }
        public string Address { get; }
        public double AddressLatitude { get; }
        public double AddressLongitude { get; }
        public string Note { get; }

        public UpdateSupplier(Guid id, string addressName, string email, string phone
            , string address, double addressLatitude, double addressLongitude, string note,
            Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            AddressName = addressName;
            Email = email;
            Phone = phone;
            Address = address;
            AddressLatitude = addressLatitude;
            AddressLongitude = addressLongitude;
            Note = note;
        }
    }

    public class DeleteSupplier : AdminBaseCommand {
        public Guid Id { get; }

        public DeleteSupplier(Guid id, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }
    }

    public class AddSuppliersToProduct :AdminBaseCommand{
        public Guid ProductId { get; }
        public List<Guid> SupplierIds { get; }

        public AddSuppliersToProduct(Guid productId, List<Guid> supplierIds, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            ProductId = productId;
            SupplierIds = supplierIds;
        }
    }
}
