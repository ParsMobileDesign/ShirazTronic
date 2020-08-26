using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic.CompilerServices;
using ShirazTronic.Data;

namespace ShirazTronic.Models
{
    public class AppUser: IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
    public class CompanyInfos
    {
        public CompanyInfos()
        {
            Title = "";Address = "";EMail = "";TelNo = "";Description = "";
        }
        [Key]
        public byte Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Address { get; set; }
        public string EMail { get; set; }
        public string TelNo { get; set; }
        public string Description { get; set; }

    }
    public class Category
    {
        public Category()
        {
            Title = "";
            Picture = new byte[byte.MaxValue];
        }

        [Key]
        [Display(Name = "Srl")]
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        [Display(Name = "Category Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Picture")]
        public byte[] Picture { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }

    }
    public class SubCategory
    {
        public SubCategory()
        {
            Title = "";
            Picture = new byte[byte.MaxValue];
        }
        [Key]
        [Display(Name = "Srl")]
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Picture")]
        public byte[] Picture { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
    #region Specification
    public class Specification
    {
        public Specification()
        {
            this.Title = "";
            this.DisplayOrder = 0;
        }
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [DisplayName("Display Order")]
        public short DisplayOrder { get; set; }
    }
    public class SpecificationValue
    {
        public SpecificationValue()
        {
            this.SpecificationId = 0;
            this.Value = "";
        }
        public int Id { get; set; }
       
        public int SpecificationId { get; set; }
        [ForeignKey("SpecificationId")]
        public virtual Specification Specification { get; set; }
        [Required]
        [MaxLength(200)]
        public string Value { get; set; }

    }
    public class ProductSpecification
    {
        public ProductSpecification()
        {
            this.ProductId = 0;
            this.SpecificationValueId = 0;
        }
        public ProductSpecification(int ProductId,int SpecValId)
        {
            this.ProductId = ProductId;
            this.SpecificationValueId = SpecValId;
        }
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public int SpecificationValueId { get; set; }
        [ForeignKey("SpecificationValueId")]
        public virtual SpecificationValue SpecificationValue { get; set; }
    }
    #endregion
    #region Product related Classes
    public class Product
    {
        public Product()
        {
            Id = 0; Title = "";ShortDescription = "";FullDescription = "";StockQuantity = 0;NotifyQuantityBelow = 0;Price = 0;
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        [Required]
        [Display(Name ="Quantity")]
        public int StockQuantity { get; set; }

        [Required]
        [Display(Name = "Notify Quantity")]
        public int NotifyQuantityBelow { get; set; }
        [Required]

        [Column(TypeName ="decimal(7,4)")]
        public decimal Price { get; set; }

        public ICollection<ProductCategory> Categories { get; set; }
        public ICollection<ProductImage> Images { get; set; }
        public ICollection<ProductSpecification> Specifications { get; set; }
    }

    public class ProductImage
    {
        public ProductImage()
        {
            this.ImageAddr="";
            this.DisplayOrder = 0;
        }
        public ProductImage(Product iProduct)
        {
            this.ProductId = iProduct.Id;
            this.Product = iProduct;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [DisplayName("Image")]
        public string ImageAddr { get; set; }
        public byte DisplayOrder { get; set; }
    }

    public class ProductCategory
    {
        public ProductCategory()
        {

        }
        public ProductCategory(Product iProduct)
        {
            this.ProductId = iProduct.Id;
            this.Product = iProduct;
        }
        [Key]
        [Display(Name = "Srl")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CatId { get; set; }
        [ForeignKey("CatId")]
        public virtual Category Category { get; set; }

        [Required]
        [Display(Name = "Sub Category")]
        public int SubCatId { get; set; }
        [ForeignKey("SubCatId")]
        public virtual SubCategory SubCategory { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
    #endregion
}
