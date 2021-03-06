﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryProject
{
    class Factory : IWorkplace
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Employee> Employees { get; set; }
        public Organisation OrganisationRelated { get; set; }
        public List<Chocolate> ChocolateWarehouse { get; set; } //Chocolate Warehouse
        public Contract ActiveContract { get; set; } //Contract to reference or resupply
        public List<Contract> RegisteredContracts { get; set; } //Contracts record
        public List<ChocolateOrder> OrdersConducted { get; set; }
        private double rawMaterial;
        public double RawMaterial
        {
            get
            {
                return rawMaterial;
            }
            set
            {
                if (value > 10000) //in units..
                {
                    throw new StockException("Value exceeded stock capacity");
                }
                else if (value < 0)
                {
                    throw new LessZeroException("Value cannot be lower than zero");
                }
                else if (value < 1000) //1000 represent 10% of 10000 -> thats when resupplying is made, incresing amount and expenses as well
                {
                    Resupply();
                }
                else
                {
                    rawMaterial = value;
                }
            }
        }


        public double Expenses { get; set; }

        public Factory(string name, double rawmaterial, Organisation organisationRelated)
        {
            Name = name;
            RawMaterial = rawmaterial;
            OrganisationRelated = organisationRelated;
            Employees = new List<Employee>();
            ChocolateWarehouse = new List<Chocolate>();
            OrdersConducted = new List<ChocolateOrder>();
        }

        /// <summary>
        /// Takes as parameteres the Store that is placing the order and the information about the chocolates to "extraxt" from the warehouse
        /// Returns a chocolate order containing a list of chocolates representing actual objects from the warehouse fot he factory
        /// </summary>
        /// <returns></returns>
        public ChocolateOrder ShipChocolateOrder(Store storeRelated, ChocolateOrder order)
        {
            //variables to keep track of number of items to remove from warehouse
            int darkChocos = 0;
            int whiteChocos = 0;
            int milkChocos = 0;
            int peanutChocos = 0;
            int almondChocos = 0;

            //Modifying valye of variables depending on data 
            foreach (var chocolate in order.Chocolates)
            {
                if (chocolate.ChocolateKind == Kind.White)
                    whiteChocos += 1;
                else if (chocolate.ChocolateKind == Kind.Peanut)
                    peanutChocos += 1;
                else if (chocolate.ChocolateKind == Kind.Milk)
                    milkChocos += 1;
                else if (chocolate.ChocolateKind == Kind.Dark)
                    darkChocos += 1;
                else //(ChocolateKind == Kind.Almond)
                    almondChocos += 1;
            }


            List<Chocolate> chocolatesList = new List<Chocolate>();//Creating the list that we will use to create the returning chocolate order


            //Querying the values from warehouse to create lists
            var almondChocolates = ChocolateListQuery(almondChocos, "Almond");
            var whiteChocolates = ChocolateListQuery(whiteChocos, "White");
            var darkChocolates = ChocolateListQuery(darkChocos, "Dark");
            var peanutChocolates = ChocolateListQuery(peanutChocos, "Peanut");
            var milkChocolates = ChocolateListQuery(milkChocos, "Milk");

            //var whiteChocolates = (from c in ChocolateWarehouse
            //                       where c.ChocolateKind == Kind.White
            //                       orderby c.DateProduced ascending
            //                       select c).Take(whiteChocos);

            //var darkChocolates = (from c in ChocolateWarehouse
            //                      where c.ChocolateKind == Kind.Dark
            //                      orderby c.DateProduced ascending
            //                      select c).Take(darkChocos);

            //var peanutChocolates = (from c in ChocolateWarehouse
            //                        where c.ChocolateKind == Kind.Peanut
            //                        orderby c.DateProduced ascending
            //                        select c).Take(peanutChocos);

            //var milkChocolates = (from c in ChocolateWarehouse
            //                      where c.ChocolateKind == Kind.Milk
            //                      orderby c.DateProduced ascending
            //                      select c).Take(milkChocos);

            //foreach (var item in almondChocolates)
            //{
            //    chocolatesList.Add(item); //Adding selected values to our list
            //    ChocolateWarehouse.Remove(item);//Removing them from warehouse
            //}

            //foreach (var item in whiteChocolates)
            //{
            //    chocolatesList.Add(item);
            //    ChocolateWarehouse.Remove(item);
            //}

            //foreach (var item in darkChocolates)
            //{
            //    chocolatesList.Add(item);
            //    ChocolateWarehouse.Remove(item);
            //}

            //foreach (var item in peanutChocolates)
            //{
            //    chocolatesList.Add(item);
            //    ChocolateWarehouse.Remove(item);
            //}

            AddRemoveChocolates(chocolatesList, milkChocolates);
            AddRemoveChocolates(chocolatesList, peanutChocolates);
            AddRemoveChocolates(chocolatesList, darkChocolates);
            AddRemoveChocolates(chocolatesList, whiteChocolates);
            AddRemoveChocolates(chocolatesList, almondChocolates);

            ChocolateOrder newOrder = new ChocolateOrder(chocolatesList, this, storeRelated);
            OrdersConducted.Add(newOrder);
            return newOrder;


        }

        private void AddRemoveChocolates(List<Chocolate> chocolatesList, IEnumerable<Chocolate> chocolatesKind)
        {
            foreach (var item in chocolatesKind)
            {
                chocolatesList.Add(item);
                ChocolateWarehouse.Remove(item);
            }
        }

        private IEnumerable<Chocolate> ChocolateListQuery(int Choco,string kind)
        {
            return (from c in ChocolateWarehouse
                    where c.ChocolateKind == (Kind)Enum.Parse(typeof(Kind),kind,true)
                    orderby c.DateProduced ascending
                    select c).Take(Choco);
        }

        public void UpdateContract(Contract newContract)
        {
            ActiveContract = newContract;
            RegisteredContracts.Add(newContract);
        }

        public void ProduceChoocolate(int dark, int white, int milk, int almond, int peanut)
        {
            ProduceChocoByKind(dark);
            ProduceChocoByKind(milk);
            ProduceChocoByKind(white);
            ProduceChocoByKind(almond);
            ProduceChocoByKind(peanut);

            //for (int i = 0; i < white; i++)
            //{
            //    Chocolate whiteChocolate = new Chocolate(Kind.White, DateTime.Now);
            //    RawMaterial -= 0.2;
            //    ChocolateWarehouse.Add(whiteChocolate);
            //}

            //for (int i = 0; i < milk; i++)
            //{
            //    Chocolate milkChocolate = new Chocolate(Kind.Milk, DateTime.Now);
            //    RawMaterial -= 0.25;
            //    ChocolateWarehouse.Add(milkChocolate);
            //}

            //for (int i = 0; i < almond; i++)
            //{
            //    Chocolate almondChocolate = new Chocolate(Kind.Almond, DateTime.Now);
            //    RawMaterial -= 0.22;
            //    ChocolateWarehouse.Add(almondChocolate);
            //}

            //for (int i = 0; i < peanut; i++)
            //{
            //    Chocolate peanutChocolate = new Chocolate(Kind.Peanut, DateTime.Now);
            //    RawMaterial -= 0.26;
            //    ChocolateWarehouse.Add(peanutChocolate);
            //}
        }

        private void ProduceChocoByKind(int kind)
        {
            for (int i = 0; i < kind; i++)
            {
                Chocolate chocolate = new Chocolate((Kind)Enum.Parse(typeof(Kind),nameof(kind),true), DateTime.Now);
                RawMaterial -= 0.15;
                ChocolateWarehouse.Add(chocolate);
            }
        }

        public void Resupply()
        {
            rawMaterial += ActiveContract.RelatedOffer.RawMaterialAmount;//Contract holds the "transaction" data to be transfered
            Expenses += ActiveContract.RelatedOffer.PricePerKilo;
        }

    }
}
