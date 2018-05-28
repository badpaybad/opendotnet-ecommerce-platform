using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class RemoveImagesFromProduct : AdminBaseCommand
    {
        public Guid Id { get; }
        public List<string> UrlImages { get; }

        public RemoveImagesFromProduct(Guid id, List<string> urlImages
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            UrlImages = urlImages;
        }

    }
}