using System;
using System.Collections.Generic;

namespace NovaMart
{
    // ---------- Custom Exceptions ----------

    public class OutOfStockException : Exception
    {
        public OutOfStockException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // ---------- Domain Models ----------

    public class Product
    {
        public int Id { get; }
        public string Name { get; }
        public double Price { get; }
        public int Stock { get; set; }

        public Product(int id, string name, double price, int stock)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
        }
    }

    public class CartItem
    {
        public Product Product { get; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    // ---------- Shopping Cart ----------

    public class ShoppingCart
    {
        private List<CartItem> cartItems = new List<CartItem>();

        public void AddItem(Product product, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidQuantityException("Quantity must be greater than zero");

            if (quantity > product.Stock)
                throw new OutOfStockException("Not enough stock available!");

            var existing = cartItems.Find(x => x.Product.Id == product.Id);

            if (existing != null)
            {
                if (existing.Quantity + quantity > product.Stock)
                    throw new OutOfStockException("Exceeding available stock!");

                existing.Quantity += quantity;
            }
            else
            {
                cartItems.Add(new CartItem(product, quantity));
            }

            Console.WriteLine("Item added to cart successfully!");
        }

        // Overloaded Method
        public void AddItem(int productId, int quantity, Dictionary<int, Product> catalog)
        {
            if (!catalog.ContainsKey(productId))
            {
                Console.WriteLine("Invalid Product ID");
                return;
            }

            AddItem(catalog[productId], quantity);
        }

        public void RemoveItem(int productId)
        {
            var item = cartItems.Find(x => x.Product.Id == productId);

            if (item != null)
            {
                cartItems.Remove(item);
                Console.WriteLine("Item removed from cart!");
            }
            else
            {
                Console.WriteLine("Item not found in cart!");
            }
        }

        public double CalculateTotal()
        {
            double total = 0;

            foreach (var item in cartItems)
            {
                total += item.Product.Price * item.Quantity;
            }

            return total;
        }

        public void DisplayCart()
        {
            Console.WriteLine("\n---- Your Cart ----");

            if (cartItems.Count == 0)
            {
                Console.WriteLine("Cart is empty!");
                return;
            }

            foreach (var item in cartItems)
            {
                Console.WriteLine($"ID: {item.Product.Id} | {item.Product.Name} | Qty: {item.Quantity} | Subtotal: {item.Product.Price * item.Quantity}");
            }

            Console.WriteLine($"Grand Total: {CalculateTotal()}\n");
        }

        public List<CartItem> GetItems()
        {
            return cartItems;
        }

        public void ClearCart()
        {
            cartItems.Clear();
        }
    }

    // ---------- Main Program ----------

    class Program
    {
        static Dictionary<int, Product> catalog = new Dictionary<int, Product>()
        {
            {1, new Product(1, "Wireless Mouse", 799.99, 20)},
            {2, new Product(2, "Keyboard", 999.50, 15)},
            {3, new Product(3, "USB Cable", 199.99, 50)},
            {4, new Product(4, "Laptop Stand", 1299.00, 10)}
        };

        static ShoppingCart cart = new ShoppingCart();

        static void Main(string[] args)
        {
            int choice = 0;

            while (choice != 6)
            {
                Console.WriteLine("\n===== Nova Mart Shopping Cart =====");
                Console.WriteLine("1. View Catalog");
                Console.WriteLine("2. Add Item to Cart");
                Console.WriteLine("3. Remove Item from Cart");
                Console.WriteLine("4. View Cart");
                Console.WriteLine("5. Checkout");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");

                int.TryParse(Console.ReadLine(), out choice);

                switch (choice)
                {
                    case 1:
                        ViewCatalog();
                        break;

                    case 2:
                        AddToCart();
                        break;

                    case 3:
                        RemoveFromCart();
                        break;

                    case 4:
                        cart.DisplayCart();
                        break;

                    case 5:
                        Checkout();
                        break;

                    case 6:
                        Console.WriteLine("Thank you for shopping with Nova Mart!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
        }

        static void ViewCatalog()
        {
            Console.WriteLine("\n---- Product Catalog ----");

            foreach (var product in catalog.Values)
            {
                Console.WriteLine($"ID: {product.Id} | {product.Name} | Price: {product.Price} | Stock: {product.Stock}");
            }
        }

        static void AddToCart()
        {
            try
            {
                Console.Write("Enter Product ID: ");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Enter Quantity: ");
                int qty = int.Parse(Console.ReadLine());

                cart.AddItem(id, qty, catalog);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (OutOfStockException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch
            {
                Console.WriteLine("Invalid input!");
            }
        }

        static void RemoveFromCart()
        {
            Console.Write("Enter Product ID to remove: ");
            int id = int.Parse(Console.ReadLine());

            cart.RemoveItem(id);
        }

        static void Checkout()
        {
            var items = cart.GetItems();

            if (items.Count == 0)
            {
                Console.WriteLine("Cart is empty!");
                return;
            }

            Console.WriteLine("\n===== Checkout Summary =====");

            foreach (var item in items)
            {
                Console.WriteLine($"{item.Product.Name} | Qty: {item.Quantity} | Subtotal: {item.Product.Price * item.Quantity}");

                // Deduct stock
                item.Product.Stock -= item.Quantity;
            }

            Console.WriteLine($"Grand Total: {cart.CalculateTotal()}");

            cart.ClearCart();

            Console.WriteLine("\nPurchase Successful! Stock Updated.");
        }
    }
}