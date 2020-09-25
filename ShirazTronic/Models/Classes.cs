using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShirazTronic.Data;

namespace ShirazTronic.Models
{
    public  class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public DbInitializer(ApplicationDbContext _db, UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            db = _db;
            userManager = _userManager;
            roleManager = _roleManager;
        }
        public  async void initialize()
        {
            try
            {
                if (db.Database.GetPendingMigrations().Count() > 0)
                    db.Database.Migrate();
            }
            catch (Exception e) { }

            if (db.Roles.Any(r => r.Name == U.ManagerUser)) return;

            roleManager.CreateAsync(new IdentityRole(U.ManagerUser)).GetAwaiter().GetResult();
            roleManager.CreateAsync(new IdentityRole(U.ControllerUser)).GetAwaiter().GetResult();
            roleManager.CreateAsync(new IdentityRole(U.CustomerUser)).GetAwaiter().GetResult();

            userManager.CreateAsync(new AppUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                PhoneNumber = "09111111111",
                FName = "administrator",
                LName = "Company",
                EmailConfirmed = true
            }, "Admin50#").GetAwaiter().GetResult();

            IdentityUser user = await db.Users.FirstOrDefaultAsync(e => e.UserName == "admin@gmail.com");
            await userManager.AddToRoleAsync(user, U.ManagerUser);
        }
    }
    public class ConvertExcel
    {
        public IFormFile MyImage { set; get; }
        public string EntityType { set; get; }
        public string message { set; get; }

        public bool isCategoryChecked { set; get; }
        public bool isSpecChecked { set; get; }
        public bool isProductChecked { set; get; }
    }

    public class AppUser : IdentityUser
    {
        [MaxLength(50)]
        public string FName { get; set; }

        [MaxLength(50)]
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
            Title = ""; Address = ""; EMail = ""; TelNo = ""; Description = "";
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
        public IEnumerable<SubCatSpecification> SubCatSpecifications { get; set; }

    }

    public class SubCatSpecification
    {
        public SubCatSpecification()
        {
            this.SubCategoryId = 0;
            this.SpecificationId = 0;
        }
        public SubCatSpecification(int iSubCategoryId, int iSpecificationId)
        {
            this.SubCategoryId = iSubCategoryId;
            this.SpecificationId = iSpecificationId;
        }
        public int Id { get; set; }
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }
        public int SpecificationId { get; set; }
        [ForeignKey("SpecificationId")]
        public virtual Specification Specification { get; set; }
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
        public IEnumerable<SpecificationValue> SpecificationValues { get; set; }
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
        public ProductSpecification(int ProductId, int SpecValId)
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
            Title = ""; ShortDescription = ""; FullDescription = ""; StockQuantity = 0; NotifyQuantityBelow = 0; UnitPrice = 0; BuyingPrice = 0; PartNumber = ""; Brand = ""; RegDate = new DateTime();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int StockQuantity { get; set; }

        [Required]
        [Display(Name = "Notify Quantity")]
        public int NotifyQuantityBelow { get; set; }

        [Required]
        [DisplayName("Buying Price")]
        [Column(TypeName = "decimal(7,2)")]
        public decimal BuyingPrice { get; set; }

        [Required]
        [DisplayName("Unit Price")]
        [Column(TypeName = "decimal(7,2)")]
        public decimal UnitPrice { get; set; }

        [MaxLength(100)]
        public string Brand { get; set; }

        [Required]
        [DisplayName("Part Number")]
        [MaxLength(50)]
        public string PartNumber { get; set; }

        [DisplayName("Register Date")]
        public DateTime RegDate { get; set; }

        public ICollection<ProductCategory> Categories { get; set; }
        public IEnumerable<ProductImage> Images { get; set; }
        public ICollection<ProductSpecification> ProductSpecification { get; set; }
    }
    public class render
    {
        public render(int iCatId, int iSubCatid, int iProductId, int iSpecValId, Category icat, SubCategory isubcat, Product iproduct, SpecificationValue ispecval)
        {
            this.CatId = iCatId;
            this.SubCatId = iSubCatid;
            this.ProductId = iProductId;
            this.SpecificationValueId = iSpecValId;

            this.Category = icat;
            this.SubCategory = isubcat;
            this.Product = iproduct;
            this.SpecificationValue = ispecval;


        }
        public int CatId { get; set; }
        public Category Category { get; set; }
        public int SubCatId { get; set; }
        public SubCategory SubCategory { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SpecificationValueId { get; set; }
        public SpecificationValue SpecificationValue { get; set; }

    }

    public class ProductImage
    {
        public ProductImage()
        {
            this.ImageAddr = "";
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

    #region Shopping Part
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Range(1, short.MaxValue, ErrorMessage = "Count should be at least 1")]
        public short Count { get; set; }
    }

    public class MemOrder
    {
        public MemOrder()
        {
            UserId = "";
            Date = new DateTime();
            TransactionId = "";
            Total = 0;
            OrderStatus = 0;
            PaymentStatus = 0;
            CustomerName = "";
            CuctomerPhoneNumber = "";
            AdditionalInfos = "";
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string TransactionId { get; set; }

        [Column(TypeName = "decimal(7,4)")]
        public decimal Total { get; set; }
        public byte OrderStatus { get; set; }
        public byte PaymentStatus { get; set; }

        [MaxLength(100)]
        [DisplayName("Full name")]
        public string CustomerName { get; set; }
        [MaxLength(30)]
        [DisplayName("Phone Number")]
        public string CuctomerPhoneNumber { get; set; }

        [MaxLength(500)]
        [DisplayName("Additional Information")]
        public string AdditionalInfos { get; set; }
        public IEnumerable<MemOrderItem> OrderItems { get; set; }

    }
    public class MemOrderItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MemOrderId { get; set; }
        [ForeignKey("MemOrderId")]
        public virtual MemOrder MemOrder { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public string Title { get; set; }

        public short Count { get; set; }

        [Required]
        [DisplayName("Unit Price")]
        [Column(TypeName = "decimal(7,2)")]
        public decimal Price { get; set; }

    }
    #endregion
}
