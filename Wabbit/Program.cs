// See https://aka.ms/new-console-template for more information

using Redzen.Numerics.Distributions.Double;
using Wabbit;

var randomizer = new RabbitRandomizerService();
var rabbits = new List<Rabbit>
{
    new(randomizer, Gender.Male),
    new(randomizer, Gender.Female)
};

var coopController = new CoopController(rabbits);
while (true)
{
    coopController.SimulateDay();
    Console.WriteLine(coopController);
}