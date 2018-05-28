using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductAddedImage : IEvent
    {
        public Guid Id { get; }
        public List<string> UrlImages { get; }

        public ProductAddedImage(Guid id, List<string> urlImages)
        {
            Id = id;
            UrlImages = urlImages;
        }

        public long Version { get; set; }
    }
}