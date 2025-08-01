using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Interfaces;
using RBS.Validation;

namespace RBS.Services.Implenetation;

public class TestingService : ITestingService
{
    // for testing
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public TestingService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public ApiResponse<UserDTO> MakeAdmin(int userId, int restaurantId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return ApiResponseService<UserDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
        }
        else
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant == null)
            {
                return ApiResponseService<UserDTO>
                    .Response(null, "restaurant not found", StatusCodes.Status404NotFound);
            }
            else
            {
                user.Role = ROLES.Admin;
                user.RestaurantId = restaurantId;
                user.Restaurant = restaurant;
                _context.SaveChanges();
            
                return ApiResponseService<UserDTO>
                    .Response200(_mapper.Map<UserDTO>(user));
            }
            
        }
    }

    public ApiResponse<RestaurantDTO> AddRestaurant(AddRestaurant request)
    {
        var restaurant = _mapper.Map<Restaurant>(request);
        var validator = new RestaurantValidator();
        var result = validator.Validate(restaurant);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<RestaurantDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            
            var response = ApiResponseService<RestaurantDTO>
                .Response(_mapper.Map<RestaurantDTO>(restaurant), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<SpaceDTO> AddSpace(int restaurantId, AddSpace request)
    {
        var restaurant = _context.Restaurants
            .Include(x => x.Spaces)
            .FirstOrDefault(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<SpaceDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        var space = _mapper.Map<Space>(request);
        var validator = new SpaceValidator();
        var result = validator.Validate(space);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<SpaceDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            restaurant.Spaces.Add(space);
            _context.SaveChanges();
            
            var response = ApiResponseService<SpaceDTO>
                .Response(_mapper.Map<SpaceDTO>(space), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<TableDTO> AddTable(int spaceId, AddTable request)
    {
        var space = _context.Spaces
            .Include(x => x.Tables)
            .FirstOrDefault(x => x.Id == spaceId);

        if (space == null)
        {
            var response = ApiResponseService<TableDTO>
                .Response(null, "Space not found", StatusCodes.Status404NotFound);
            return response;
        }
        var table = _mapper.Map<Table>(request);
        var validator = new TableValidator();
        var result = validator.Validate(table);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<TableDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            space.Tables.Add(table);
            _context.SaveChanges();
            
            var response = ApiResponseService<TableDTO>
                .Response(_mapper.Map<TableDTO>(table), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<ChairDTO> AddChair(int tableId, AddChair request)
    {
        var table = _context.Tables
            .Include(x => x.Chairs)
            .FirstOrDefault(x => x.Id == tableId);

        if (table == null)
        {
            var response = ApiResponseService<ChairDTO>
                .Response(null, "Table not found", StatusCodes.Status404NotFound);
            return response;
        }
        var chair = _mapper.Map<Chair>(request);
        var validator = new ChairValidator();
        var result = validator.Validate(chair);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<ChairDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            table.Chairs.Add(chair);
            _context.SaveChanges();
            
            var response = ApiResponseService<ChairDTO>
                .Response(_mapper.Map<ChairDTO>(chair), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public async Task<ApiResponse<MenuDTO>> AddFullMenu(int restaurantId)
    {
        var restaurant = await _context.Restaurants
            .Include(x => x.Menu)
            .FirstOrDefaultAsync(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<MenuDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            Menu newMenu = new Menu
            {
                Name = $"{restaurant.Title}'s Menu",
                RestaurantId = restaurantId,
                Categories = new List<FoodCategory>
                {
                    new FoodCategory
                    {
                        CategoryEnglishName = "Breakfast",
                        CategoryGeorgianName = "საუზმე",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Oatmeal",
                                GeorgianName = "შვრიის ფაფა",
                                Price = 23,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Milk", GeorgianName = "რძე" },
                                    new Ingredient { EnglishName = "Vegan Milk", GeorgianName = "ვეგანური რძე" },
                                    new Ingredient { EnglishName = "Granola", GeorgianName = "გრანოლა" },
                                    new Ingredient { EnglishName = "Fruit", GeorgianName = "ხილი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Smoothie",
                                GeorgianName = "სმუზი",
                                Price = 19,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Apple", GeorgianName = "ვაშლი" },
                                    new Ingredient { EnglishName = "Banana", GeorgianName = "ბანანი" },
                                    new Ingredient { EnglishName = "Berries", GeorgianName = "კენკრა" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Avocado Sandwich",
                                GeorgianName = "ავოკადოს სენდვიჩი",
                                Price = 38,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Avocado", GeorgianName = "ავოკადო" },
                                    new Ingredient { EnglishName = "Boiled Egg", GeorgianName = "მოხარშული კვერცხი" },
                                    new Ingredient { EnglishName = "Cherry Tomato", GeorgianName = "ჩერი პომიდორი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Salmon Sandwich",
                                GeorgianName = "ორაგულის სენდვიჩი",
                                Price = 58,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Salmon", GeorgianName = "ორაგული" },
                                    new Ingredient { EnglishName = "Egg", GeorgianName = "კვერცხი" },
                                    new Ingredient { EnglishName = "Green Onion", GeorgianName = "მწვანე ხახვი" },
                                    new Ingredient { EnglishName = "Cream Cheese", GeorgianName = "კრემყველი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Vegan/Vegetarian Bowl",
                                GeorgianName = "ვეგანური /ვეგეტარიანული ჯამი",
                                Price = 38,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Egg", GeorgianName = "კვერცხი" },
                                    new Ingredient { EnglishName = "Broccoli", GeorgianName = "ბროკოლი" },
                                    new Ingredient { EnglishName = "Quinoa", GeorgianName = "ქინოა" },
                                    new Ingredient { EnglishName = "Avocado", GeorgianName = "ავოკადო" },
                                    new Ingredient { EnglishName = "Cherry Tomato", GeorgianName = "ჩერი პომიდორი" },
                                    new Ingredient { EnglishName = "Red Cabbage", GeorgianName = "წითელი კომბოსტო" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Chicken Bowl",
                                GeorgianName = "ქათმის ჯამი",
                                Price = 38,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Egg", GeorgianName = "კვერცხი" },
                                    new Ingredient { EnglishName = "Broccoli", GeorgianName = "ბროკოლი" },
                                    new Ingredient { EnglishName = "Quinoa", GeorgianName = "ქინოა" },
                                    new Ingredient { EnglishName = "Chicken Fillet", GeorgianName = "ქათმის ფილე" },
                                    new Ingredient { EnglishName = "Cherry Tomato", GeorgianName = "ჩერი პომიდორი" },
                                    new Ingredient { EnglishName = "Red Cabbage", GeorgianName = "წითელი კომბოსტო" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Salmon Bowl",
                                GeorgianName = "ორაგულის ჯამი",
                                Price = 58,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Egg", GeorgianName = "კვერცხი" },
                                    new Ingredient { EnglishName = "Broccoli", GeorgianName = "ბროკოლი" },
                                    new Ingredient { EnglishName = "Quinoa", GeorgianName = "ქინოა" },
                                    new Ingredient { EnglishName = "Salmon", GeorgianName = "ორაგული" },
                                    new Ingredient { EnglishName = "Cucumber", GeorgianName = "კიტრი" },
                                    new Ingredient { EnglishName = "Cream Cheese", GeorgianName = "კრემყველი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Turkey Sandwich",
                                GeorgianName = "ინდაურის სენდვიჩი",
                                Price = 23,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Turkey", GeorgianName = "ინდაური" },
                                    new Ingredient { EnglishName = "Croissant", GeorgianName = "კრუასანი" },
                                    new Ingredient { EnglishName = "Cream Cheese", GeorgianName = "კრემყველი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Bread Basket",
                                GeorgianName = "პურის კალათა",
                                Price = 35,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Butter", GeorgianName = "კარაქი" },
                                    new Ingredient { EnglishName = "Honey", GeorgianName = "თაფლი" },
                                    new Ingredient { EnglishName = "Jam", GeorgianName = "ჯემი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Pear Dessert",
                                GeorgianName = "მსხლის დესერტი",
                                Price = 27,
                                FoodType = FOOD_TYPE.Regular
                            },
                            new Food
                            {
                                EnglishName = "Apple Strudel with Walnuts",
                                GeorgianName = "ნიგვზიანი ვაშლის შტრუდელი",
                                Price = 29,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Apple", GeorgianName = "ვაშლი" },
                                    new Ingredient { EnglishName = "Walnuts", GeorgianName = "ნიგვზი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Fruit Tart",
                                GeorgianName = "ხილის ტარტი",
                                Price = 25,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Fruit", GeorgianName = "ხილი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Chia Pudding",
                                GeorgianName = "ჩიას პუდინგი",
                                Price = 17,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Chia", GeorgianName = "ჩია" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Croissant",
                                GeorgianName = "კრუასანი",
                                Price = 5,
                                FoodType = FOOD_TYPE.Regular
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Soups",
                        CategoryGeorgianName = "წვნიანი",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Lentil Soup",
                                GeorgianName = "ოსპის წვნიანი",
                                Price = 20,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Dark Bread Croutons", GeorgianName = "გახუხული პური" },
                                    new Ingredient { EnglishName = "Lemon", GeorgianName = "ლიმონი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Chicken Soup",
                                GeorgianName = "ქათმის წვნიანი",
                                Price = 25,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Chinese Cabbage", GeorgianName = "ჩინური კომბოსტო" },
                                    new Ingredient { EnglishName = "Wild Mushrooms", GeorgianName = "ტყის სოკო" },
                                    new Ingredient { EnglishName = "Rice Noodles", GeorgianName = "ბრინჯის ატრია" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Beef Soup",
                                GeorgianName = "საქონლის წვნიანი",
                                Price = 27,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Chinese Cabbage", GeorgianName = "ჩინური კომბოსტო" },
                                    new Ingredient { EnglishName = "Wild Mushrooms", GeorgianName = "ტყის სოკო" },
                                    new Ingredient { EnglishName = "Rice Noodles", GeorgianName = "ბრინჯის ატრია" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Salad",
                        CategoryGeorgianName = "სალათი",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Cherry Tomato Salad",
                                GeorgianName = "ბაღის კიტრი -პომიდვრის სალათი",
                                Price = 27,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Walnut Sauce", GeorgianName = "ნიგვზის სოუსი" },
                                    new Ingredient { EnglishName = "Pesto Sauce", GeorgianName = "პესტოს სოუსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Salmon Salad",
                                GeorgianName = "ორაგულის სალათი",
                                Price = 59,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Salmon", GeorgianName = "ორაგული" },
                                    new Ingredient
                                        { EnglishName = "Citrus Dressing", GeorgianName = "ციტრუსის დრესინგი" },
                                    new Ingredient { EnglishName = "Sesame Seeds", GeorgianName = "სეზამის მარცვლები" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Steak Salad",
                                GeorgianName = "სტეიკ სალათი",
                                Price = 59,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                    {
                                        EnglishName = "Grilled Beef", GeorgianName = "გრილზე შემწვარი საქონლის ხორცი"
                                    },
                                    new Ingredient { EnglishName = "Mushrooms", GeorgianName = "სოკო" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Cheese Salad",
                                GeorgianName = "ყველის სალათი",
                                Price = 49,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Grilled Pear", GeorgianName = "გრილზე შემწვარი მსხალი" },
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Pumpkin Salad",
                                GeorgianName = "გოგრის სალათი",
                                Price = 39,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Pumpkin", GeorgianName = "გოგრა" },
                                    new Ingredient { EnglishName = "Walnuts", GeorgianName = "ნიგვზი" },
                                    new Ingredient { EnglishName = "Prunes", GeorgianName = "შავი ქლიავი" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Boards and Appetizers",
                        CategoryGeorgianName = "დაფა წასახემსებელი",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Cheese Board",
                                GeorgianName = "ყველის დაფა",
                                Price = 65,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "European Cheese", GeorgianName = "ევროპული ყველი" },
                                    new Ingredient { EnglishName = "Georgian Cheese", GeorgianName = "ქართული ყველი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Hummus Board",
                                GeorgianName = "ჰუმუსის დაფა",
                                Price = 36,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Vegetables Sticks", GeorgianName = "ბოსტნეულის ჩხირები" },
                                    new Ingredient { EnglishName = "Bread", GeorgianName = "პური" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Olive Board",
                                GeorgianName = "ზეთისხილის დაფა",
                                Price = 32,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Olives", GeorgianName = "ზეთისხილი" },
                                    new Ingredient { EnglishName = "Olive Puree", GeorgianName = "ზეთისხილის პიურე" },
                                    new Ingredient { EnglishName = "Bread", GeorgianName = "პური" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Fish",
                        CategoryGeorgianName = "თევზეული",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Trout",
                                GeorgianName = "კალმახი",
                                Price = 41,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Vegetables", GeorgianName = "ბოსტნეული" },
                                    new Ingredient
                                        { EnglishName = "Worcestershire Sauce", GeorgianName = "ვოლჩესტერის სოუსი" },
                                    new Ingredient { EnglishName = "Mustard", GeorgianName = "ნაშარაფი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Dorado",
                                GeorgianName = "დორადო",
                                Price = 67,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Rice", GeorgianName = "ბრინჯი" },
                                    new Ingredient { EnglishName = "Vegetable Mix", GeorgianName = "ბოსტნეულის მიქსი" },
                                    new Ingredient { EnglishName = "Cherry Sauce", GeorgianName = "ალუბლის სოუსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Salmon Steak",
                                GeorgianName = "ორაგულის სტეიკი",
                                Price = 76,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Avocado", GeorgianName = "ავოკადო" },
                                    new Ingredient { EnglishName = "Asparagus", GeorgianName = "სატაცური" },
                                    new Ingredient { EnglishName = "Sweet Potato", GeorgianName = "ბატატი" },
                                    new Ingredient { EnglishName = "Carrot Puree", GeorgianName = "სტაფილოს პიურე" },
                                    new Ingredient { EnglishName = "Citrus Sauce", GeorgianName = "ციტრუსის სოუსი" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Burgers",
                        CategoryGeorgianName = "ბურგერები",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Classic Vegan Burger",
                                GeorgianName = "ვეგან ბურგერი",
                                Price = 35,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Black Beans Patty", GeorgianName = "შავი ლობიოს კატლეტი" },
                                    new Ingredient { EnglishName = "Lettuce", GeorgianName = "კომბოსტოს ფოთოლი" },
                                    new Ingredient { EnglishName = "Onion", GeorgianName = "ხახვი" },
                                    new Ingredient { EnglishName = "Tomato", GeorgianName = "პომიდორი" },
                                    new Ingredient
                                    {
                                        EnglishName = "Pickled Cucumber", GeorgianName = "მარინადში ჩაწყვეტილი კიტრი"
                                    },
                                    new Ingredient { EnglishName = "Ketchup", GeorgianName = "კეჩუპი" },
                                    new Ingredient { EnglishName = "French Fries", GeorgianName = "კარტოფილი ფრი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Vegetarian Cheese Burger",
                                GeorgianName = "ვეგეტარიანული ჩიზბურგერი",
                                Price = 39,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Black Beans Patty", GeorgianName = "შავი ლობიოს კატლეტი" },
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient
                                    {
                                        EnglishName = "Mayo Based Sauce",
                                        GeorgianName = "მაიონეზის ბაზაზე დამზადებული სოუსი"
                                    },
                                    new Ingredient { EnglishName = "Lettuce", GeorgianName = "კომბოსტოს ფოთოლი" },
                                    new Ingredient { EnglishName = "Onion", GeorgianName = "ხახვი" },
                                    new Ingredient { EnglishName = "Tomato", GeorgianName = "პომიდორი" },
                                    new Ingredient { EnglishName = "French Fries", GeorgianName = "კარტოფილი ფრი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Falafel Burger",
                                GeorgianName = "ფალაფელ ბურგერი",
                                Price = 35,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient
                                        { EnglishName = "Falafel Patty", GeorgianName = "ფალაფელის კატლეტი" },
                                    new Ingredient { EnglishName = "Lettuce", GeorgianName = "კომბოსტოს ფოთოლი" },
                                    new Ingredient { EnglishName = "Onion", GeorgianName = "ხახვი" },
                                    new Ingredient { EnglishName = "Tomato", GeorgianName = "პომიდორი" },
                                    new Ingredient
                                        { EnglishName = "Almond Butter Sauce", GeorgianName = "ნუშის კარაქის სოუსი" },
                                    new Ingredient { EnglishName = "French Fries", GeorgianName = "კარტოფილი ფრი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Chicken Cheese Burger",
                                GeorgianName = "ქათმის ჩიზბურგერი",
                                Price = 48,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Chicken Fillet", GeorgianName = "ქათმის ფილე" },
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient { EnglishName = "Lettuce", GeorgianName = "კომბოსტოს ფოთოლი" },
                                    new Ingredient { EnglishName = "Onion", GeorgianName = "ხახვი" },
                                    new Ingredient { EnglishName = "Tomato", GeorgianName = "პომიდორი" },
                                    new Ingredient
                                    {
                                        EnglishName = "Pickled Cucumber", GeorgianName = "მარინადში ჩაწყვეტილი კიტრი"
                                    },
                                    new Ingredient { EnglishName = "Ketchup", GeorgianName = "კეჩუპი" },
                                    new Ingredient { EnglishName = "Mayonnaise", GeorgianName = "მაიონეზი" },
                                    new Ingredient { EnglishName = "French Fries", GeorgianName = "კარტოფილი ფრი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Beef Cheese Burger",
                                GeorgianName = "საქონლის ჩიზბურგერი",
                                Price = 52,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient { EnglishName = "Lettuce", GeorgianName = "კომბოსტოს ფოთოლი" },
                                    new Ingredient { EnglishName = "Onion", GeorgianName = "ხახვი" },
                                    new Ingredient { EnglishName = "Bacon", GeorgianName = "ბეკონი" },
                                    new Ingredient { EnglishName = "Tomato", GeorgianName = "პომიდორი" },
                                    new Ingredient
                                    {
                                        EnglishName = "Pickled Cucumber", GeorgianName = "მარინადში ჩაწყვეტილი კიტრი"
                                    },
                                    new Ingredient { EnglishName = "Ketchup", GeorgianName = "კეჩუპი" },
                                    new Ingredient { EnglishName = "Mayonnaise", GeorgianName = "მაიონეზი" },
                                    new Ingredient { EnglishName = "French Fries", GeorgianName = "კარტოფილი ფრი" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Pizza",
                        CategoryGeorgianName = "პიცა",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Margarita",
                                GeorgianName = "მარგარიტა",
                                Price = 36,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Tomato Sauce", GeorgianName = "პომიდვრის სოუსი" },
                                    new Ingredient { EnglishName = "Basil", GeorgianName = "ბაზილიკი" },
                                    new Ingredient
                                        { EnglishName = "Mozzarella Cheese", GeorgianName = "მოცარელას ყველი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Pepperoni",
                                GeorgianName = "პეპერონი",
                                Price = 45,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Tomato Sauce", GeorgianName = "პომიდვრის სოუსი" },
                                    new Ingredient { EnglishName = "Basil", GeorgianName = "ბაზილიკი" },
                                    new Ingredient
                                        { EnglishName = "Mozzarella Cheese", GeorgianName = "მოცარელას ყველი" },
                                    new Ingredient
                                        { EnglishName = "Spicy Pepperoni Sausage", GeorgianName = "პეპერონის ძეხვი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Capricciosa",
                                GeorgianName = "კაპრიჩოზა",
                                Price = 45,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Tomato Sauce", GeorgianName = "პომიდვრის სოუსი" },
                                    new Ingredient { EnglishName = "Basil", GeorgianName = "ბაზილიკი" },
                                    new Ingredient
                                        { EnglishName = "Mozzarella Cheese", GeorgianName = "მოცარელას ყველი" },
                                    new Ingredient { EnglishName = "Ham", GeorgianName = "ღამბო" },
                                    new Ingredient { EnglishName = "Mushrooms", GeorgianName = "სოკო" },
                                    new Ingredient { EnglishName = "Olives", GeorgianName = "ზეთისხილი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Capricciosa",
                                GeorgianName = "კაპრიჩოზა",
                                Price = 45,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Tomato Sauce", GeorgianName = "პომიდვრის სოუსი" },
                                    new Ingredient { EnglishName = "Basil", GeorgianName = "ბაზილიკი" },
                                    new Ingredient
                                        { EnglishName = "Mozzarella Cheese", GeorgianName = "მოცარელას ყველი" },
                                    new Ingredient { EnglishName = "Ham", GeorgianName = "ღამბო" },
                                    new Ingredient { EnglishName = "Mushrooms", GeorgianName = "სოკო" },
                                    new Ingredient { EnglishName = "Olives", GeorgianName = "ზეთისხილი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Verdura",
                                GeorgianName = "პიცა ვერდურა",
                                Price = 31,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Tomato Sauce", GeorgianName = "პომიდვრის სოუსი" },
                                    new Ingredient { EnglishName = "Assortment of Vegetables", GeorgianName = "ბოსტნეული" },
                                }
                            },  
                        },
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Baked Goods",
                        CategoryGeorgianName = "ცომეული",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Lobiani",
                                GeorgianName = "ლობიანი",
                                Price = 32,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Beans", GeorgianName = "ლობიო" },
                                    new Ingredient { EnglishName = "Bread", GeorgianName = "პური" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Khatchapuri",
                                GeorgianName = "ხაჭაპური",
                                Price = 38,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient { EnglishName = "Bread", GeorgianName = "პური" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Adjarian Khachapuri",
                                GeorgianName = "აჭარული ხაჭაპური",
                                Price = 36,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient { EnglishName = "Egg", GeorgianName = "კვერცხი" },
                                    new Ingredient { EnglishName = "Butter", GeorgianName = "კარაქი" },
                                    new Ingredient { EnglishName = "Bread", GeorgianName = "პური" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Bread Basket",
                                GeorgianName = "პურის ასორტი",
                                Price = 15,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Bread", GeorgianName = "პური" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Main Courses",
                        CategoryGeorgianName = "ძირითადი კერძები",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Khinkali",
                                GeorgianName = "ხინკალი",
                                Price = 36,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Mushroom Cream Sauce", GeorgianName = "სოკოს ნაღების სოუსი" },
                                    new Ingredient { EnglishName = "Pesto Sauce", GeorgianName = "პესტოს სოუსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Cheese and Potato Khinkali",
                                GeorgianName = "ყველის და კარტოფილის კვერები",
                                Price = 30,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient { EnglishName = "Yogurt Sauce", GeorgianName = "მაწვნის სოუსი" },
                                    new Ingredient { EnglishName = "Butter", GeorgianName = "კარაქი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Mushrooms on the Grill",
                                GeorgianName = "სოკო გრილზე",
                                Price = 36,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Mushrooms", GeorgianName = "სოკო" },
                                    new Ingredient { EnglishName = "Guacamole", GeorgianName = "გვაკამოლე" },
                                    new Ingredient { EnglishName = "Yogurt Bread", GeorgianName = "იოგურტის პური" },
                                    new Ingredient { EnglishName = "Pesto Sauce", GeorgianName = "პესტოს სოუსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Cauliflower Steak",
                                GeorgianName = "ყვავილოვანი კომბოსტოს სტეიკი",
                                Price = 49,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Almond Batter", GeorgianName = "ნუშის ბაჟე" },
                                    new Ingredient { EnglishName = "Chimichurri Sauce", GeorgianName = "ჩიმიჩურის სოუსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Skhmeruli",
                                GeorgianName = "შქმერული",
                                Price = 49,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Chicken Fillet", GeorgianName = "ქათმის ფილე" },
                                    new Ingredient { EnglishName = "Garlic Cream Sauce", GeorgianName = "ნივრისა და ნაღების სოუსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Chicken Dish",
                                GeorgianName = "ქათმის კერძი",
                                Price = 49,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Chicken Fillet", GeorgianName = "ქათმის ფილე" },
                                    new Ingredient { EnglishName = "Yogurt Bread", GeorgianName = "იოგურტის პური" },
                                    new Ingredient { EnglishName = "Rice", GeorgianName = "ბრინჯი" },
                                    new Ingredient { EnglishName = "Cream", GeorgianName = "ნაღები" },
                                    new Ingredient { EnglishName = "Tomato Sauce", GeorgianName = "პომიდვრის სოუსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Pork Entrecot",
                                GeorgianName = "ღორის ანტრიკოტი",
                                Price = 69,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Wine Sauce", GeorgianName = "ღვინის სოუსი" },
                                    new Ingredient { EnglishName = "French Fries", GeorgianName = "კარტოფილი ფრი" },
                                    new Ingredient { EnglishName = "Pickled Cucumber", GeorgianName = "მარინადში ჩაწყვეტილი კიტრი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Roasted Beef Cheek",
                                GeorgianName = "გამომცხვარი საქონლის ლოყა",
                                Price = 74,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Sweet Potato", GeorgianName = "ბატატი" },
                                    new Ingredient { EnglishName = "Vegetable Mix", GeorgianName = "ბოსტნეულის მიქსი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Vegetable Ragout",
                                GeorgianName = "ბოსტნეულის რაგუ",
                                Price = 32,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Broccoli", GeorgianName = "ბროკოლი" },
                                    new Ingredient { EnglishName = "Vegetables", GeorgianName = "ბოსტნეული" },
                                    new Ingredient { EnglishName = "Mushrooms", GeorgianName = "სოკო" },
                                    new Ingredient { EnglishName = "Pumpkin", GeorgianName = "გოგრა" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Beans in a Pot",
                                GeorgianName = "ლობიო ქოთანში",
                                Price = 32,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Beans", GeorgianName = "ლობიო" },
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient { EnglishName = "Pickled Cucumber", GeorgianName = "მარინადში ჩაწყვეტილი კიტრი" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "From The Grill",
                        CategoryGeorgianName = "გრილი",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Grilled Chicken",
                                GeorgianName = "ქათმის მწვადი",
                                Price = 35,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Chicken Fillet", GeorgianName = "ქათმის ფილე" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Grilled Pork",
                                GeorgianName = "ღორის მწვადი",
                                Price = 39,
                                FoodType = FOOD_TYPE.Regular
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Pasta",
                        CategoryGeorgianName = "პასტა",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Carbonara",
                                GeorgianName = "კარბონარა",
                                Price = 39,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Bacon", GeorgianName = "ბეკონი" },
                                    new Ingredient { EnglishName = "Cream", GeorgianName = "ნაღები" },
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" },
                                    new Ingredient { EnglishName = "Egg", GeorgianName = "კვერცხი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Fettuccine Alfredo, Chicken and Mushroom",
                                GeorgianName = "ფეტუჩინი ქათმით და სოკოთი",
                                Price = 39,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Chicken Fillet", GeorgianName = "ქათმის ფილე" },
                                    new Ingredient { EnglishName = "Mushrooms", GeorgianName = "სოკო" },
                                    new Ingredient { EnglishName = "Cream", GeorgianName = "ნაღები" },
                                    new Ingredient { EnglishName = "Cheese", GeorgianName = "ყველი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Verdura",
                                GeorgianName = "ვერდურა",
                                Price = 35,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Vegetables", GeorgianName = "ბოსტნეული" },
                                    new Ingredient { EnglishName = "Tomato Sauce", GeorgianName = "პომიდვრის სოუსი" },
                                    new Ingredient { EnglishName = "Basil", GeorgianName = "ბაზილიკი" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Garnish",
                        CategoryGeorgianName = "გარნირი",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "French Fries",
                                GeorgianName = "კარტოფილი ფრი",
                                Price = 16,
                                FoodType = FOOD_TYPE.Vegan
                            },
                            new Food
                            {
                                EnglishName = "Rice",
                                GeorgianName = "ბრინჯი",
                                Price = 16,
                                FoodType = FOOD_TYPE.Vegan
                            },
                            new Food
                            {
                                EnglishName = "Quinoa",
                                GeorgianName = "ქინოა",
                                Price = 16,
                                FoodType = FOOD_TYPE.Vegan
                            },
                            new Food
                            {
                                EnglishName = "Onion Rings",
                                GeorgianName = "ხახვის რგოლები",
                                Price = 16,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Onion", GeorgianName = "ხახვი" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Sauces",
                        CategoryGeorgianName = "სოუსები",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Ketchup",
                                GeorgianName = "კეჩუპი",
                                Price = 5,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Tomato", GeorgianName = "პომიდორი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Mayonnaise",
                                GeorgianName = "მაიონეზი",
                                Price = 5,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Egg", GeorgianName = "კვერცხი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Mustard",
                                GeorgianName = "მდოგვი",
                                Price = 5,
                                FoodType = FOOD_TYPE.Vegan
                            },
                            new Food
                            {
                                EnglishName = "Pomegranate Sauce",
                                GeorgianName = "ტყემალი",
                                Price = 5,
                                FoodType = FOOD_TYPE.Vegan
                            },
                            new Food
                            {
                                EnglishName = "Georgian Plum Sauce",
                                GeorgianName = "საწებელი",
                                Price = 5,
                                FoodType = FOOD_TYPE.Vegan
                            },
                            new Food
                            {
                                EnglishName = "Georgian Spicy Tomato Sauce",
                                GeorgianName = "ნაშარაფი",
                                Price = 5,
                                FoodType = FOOD_TYPE.Vegan,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Tomato", GeorgianName = "პომიდორი" }
                                }
                            }
                        }
                    },
                    new FoodCategory
                    {
                        CategoryEnglishName = "Desserts",
                        CategoryGeorgianName = "დესერტი",
                        Foods = new List<Food>
                        {
                            new Food
                            {
                                EnglishName = "Apple Strudel",
                                GeorgianName = "ნიგვზიანი ვაშლის შტრუდელი",
                                Price = 29,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Apple", GeorgianName = "ვაშლი" },
                                    new Ingredient { EnglishName = "Walnuts", GeorgianName = "ნიგვზი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Drunk Cherry Cake",
                                GeorgianName = "მთვრალი ალუბალი",
                                Price = 37,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Cherry", GeorgianName = "ალუბალი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Pear Dessert",
                                GeorgianName = "მსხლის დესერტი",
                                Price = 27,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Grilled Pear", GeorgianName = "გრილზე შემწვარი მსხალი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Fruit Tart",
                                GeorgianName = "ხილის ტარტი",
                                Price = 29,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Fruit", GeorgianName = "ხილი" }
                                }
                            },
                            new Food
                            {
                                EnglishName = "Ice Cream",
                                GeorgianName = "ნაყინი",
                                Price = 18,
                                FoodType = FOOD_TYPE.Regular,
                                Ingredients = new List<Ingredient>
                                {
                                    new Ingredient { EnglishName = "Milk", GeorgianName = "რძე" },
                                    new Ingredient { EnglishName = "Cream", GeorgianName = "ნაღები" }
                                }
                            }
                        }
                    },
                }
            };

            restaurant.Menu = newMenu;
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<MenuDTO>
                .Response200(_mapper.Map<MenuDTO>(newMenu));
            return response;
        }
    }

    public ApiResponse<RestaurantDTO> DeleteRestaurant(int restaurantId)
    {
        var restaurant = _context.Restaurants
            .FirstOrDefault(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<RestaurantDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Restaurants.Remove(restaurant);
            _context.SaveChanges();

            var response = ApiResponseService<RestaurantDTO>
                .Response200(_mapper.Map<RestaurantDTO>(restaurant));
            return response;
        }
    }

    public ApiResponse<SpaceDTO> DeleteSpace(int spaceId)
    {
        var space = _context.Spaces
            .FirstOrDefault(x => x.Id == spaceId);

        if (space == null)
        {
            var response = ApiResponseService<SpaceDTO>
                .Response(null, "Space not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Spaces.Remove(space);
            _context.SaveChanges();

            var response = ApiResponseService<SpaceDTO>
                .Response200(_mapper.Map<SpaceDTO>(space));
            return response;
        }
    }

    public ApiResponse<TableDTO> DeleteTable(int tableId)
    {
        var table = _context.Tables
            .FirstOrDefault(x => x.Id == tableId);

        if (table == null)
        {
            var response = ApiResponseService<TableDTO>
                .Response(null, "Table not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Tables.Remove(table);
            _context.SaveChanges();

            var response = ApiResponseService<TableDTO>
                .Response200(_mapper.Map<TableDTO>(table));
            return response;
        }
    }

    public ApiResponse<ChairDTO> DeleteChair(int chairId)
    {
        var chair = _context.Chairs
            .FirstOrDefault(x => x.Id == chairId);

        if (chair == null)
        {
            var response = ApiResponseService<ChairDTO>
                .Response(null, "Chair not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Chairs.Remove(chair);
            _context.SaveChanges();

            var response = ApiResponseService<ChairDTO>
                .Response200(_mapper.Map<ChairDTO>(chair));
            return response;
        }
    }

}