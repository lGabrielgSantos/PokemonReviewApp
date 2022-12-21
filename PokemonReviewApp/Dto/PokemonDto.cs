namespace PokemonReviewApp.Dto
{
    public class PokemonDto
    {
        //limit data with you client acess, of your model
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
