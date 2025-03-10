using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Coffee.Data;
using Coffee.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Coffee.Pages
{
 public class OrderModel : PageModel
 {
 private CoffeeContext db;
 public OrderModel(CoffeeContext db) => this.db = db;
 [BindProperty(SupportsGet =true)]
 public int Id { get; set; }
 public Product Product { get; set;}
 [BindProperty, EmailAddress, Required, Display(Name="Your Email Address")]
 public string OrderEmail { get; set; }
 [BindProperty, Required(ErrorMessage="Please input your shipping address"), Display(Name="Shipping Address")]
 public string OrderShipping { get; set; } 
 [BindProperty, Display(Name="Quantity")]
 public int OrderQuantity { get; set; } = 1;

 public async Task OnGetAsync() => Product = await db.Products.FindAsync(Id);
  public async Task<IActionResult> OnPostAsync()
     {
        Product = await db.Products.FindAsync(Id);
        if(ModelState.IsValid)
        {
            var body = $@"<p>Thank you, we have received your order 
            for {OrderQuantity} unit(s) of {Product.Name}!</p>
            <p>Your address is: <br/>{OrderShipping.Replace("\n", 
            "<br/>")}</p>
            Your total is Rp.{Product.Price * OrderQuantity}.<br/>
            We will contact you if we have questions about your 
            order. Thanks!<br/>";
            using(var smtp = new SmtpClient())
            {
            smtp.DeliveryMethod = 
            SmtpDeliveryMethod.SpecifiedPickupDirectory;
            smtp.PickupDirectoryLocation = @"C:\emailOrder";
            var message = new MailMessage();
            message.To.Add(OrderEmail);
            message.Subject = "Coffee - New Order";
            message.Body = body;
            message.IsBodyHtml = true;
            message.From = new MailAddress("sales@Coffee.com");
            await smtp.SendMailAsync(message);
            }

            return RedirectToPage("OrderSuccess");
        }
    return Page();
     }
 }
}

