namespace Butik
{
    class CartProduct
    {
        public string Name { get; }
        public int Quantity { get; private set; }
        public double Cost { get; private set; }

        // Prevents the Price-variable to be shown in the cart DataGridView.
        [System.ComponentModel.Browsable(false)]
        public double Price { get; }

        public CartProduct(string name, int quantity, double price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
            Cost = Quantity * Price;
        }

        public void IncreaseQuantity()
        {
            Quantity++;
            Cost += Price;
        }

        public void DecreaseQuantity()
        {               
            Quantity--;          
            Cost -= Price;
        }
    }
}
