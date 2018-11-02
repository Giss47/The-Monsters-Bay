namespace Butik
{
    class CartProduct
    {
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public double Cost { get; private set; }

        [System.ComponentModel.Browsable(false)]
        public double Price { get; private set; }

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
        }

        public void RecalculateCost()
        {
            Cost = Quantity * Price;
        }
    }
}
