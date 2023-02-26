using Bogus;
using Lab1;
using Lab1.Models;
using Lab1.Models.Enums;
using System.Text;
Faker faker = new Faker();
Console.OutputEncoding = Encoding.UTF8;
List<string> additionalOptionsNames = new List<string> {
    "TV", "Air Conditioner", "Mini Bar", "Child Bed",
    "Safe", "Balcony", "Sea View", "Mountain View",
    "Pool View", "Garden View", "Wi-Fi",
    "Parking", "Breakfast", "Gym",
    "Spa", "Shuttle", "Laundry",
};
var opts = additionalOptionsNames.Select(x => new Option {
    Name = x,
    Price = faker.PickRandom<double>(new List<double>(){0, 10})
}).ToList();

List<Client> clients = faker.Make(20, () => new Client
{
    Name = faker.Name.FirstName(),
    Surname = faker.Name.LastName(),
    Phone = faker.Phone.PhoneNumber(),
}).ToList();




HotelManager HotelManager = new HotelManager();


var rooms = new List<Room>();
foreach (var num in Enumerable.Range(1,15))
{
    rooms.Add(new Room()
    {
        Number = num,
        Capacity = faker.Random.Int(1, 6),
        Class = faker.PickRandom<RoomClass>(),
        Options = faker.PickRandom(opts, Random.Shared.Next(0, opts.Count())).ToList()
    });
}
var bookings = new List<Booking>();
foreach (var item in Enumerable.Range(1,30))
{
    bookings.Add(new Booking() // TODO : FIX BOOKING GENERATING BY DATE
    {
        Client = faker.PickRandom(clients),
        RoomNumber = faker.PickRandom(rooms).Number,
        CheckIn = faker.Date.Between(new DateTime(2023, 1, 1), new DateTime(2023, 12, 1)),
        CheckOut = faker.Date.Between(new DateTime(2023, 2, 1), new DateTime(2024, 1, 1)),
    });
}
bookings.Sort(new Comparison<Booking>((x, y) => x.RoomNumber.CompareTo(y.RoomNumber)));
HotelManager.AddRooms(rooms.DistinctBy(x=>x.Number).ToList());
HotelManager.AddBookings(bookings);







while (true)
{
    Console.Clear();
    Console.WriteLine("--Choose an option--");
    Console.WriteLine("-1) Всі броні");
    Console.WriteLine("1) Чи є вільний номер який відповідає бажанням клієнта");
    Console.WriteLine("2) Всі номера готелю");
    Console.WriteLine("3) всі заброньовані номера на даний час");
    Console.WriteLine("4) всі вільні номера на даний час");
    Console.WriteLine("5) всі номера готелю з найбільшою кількістю безкоштовних додаткових опцій");
    Console.WriteLine("6) всі номера готелю заданого типу заброньовані на даний період");
    Console.WriteLine("7) Вивести номер номеру який бронювали найбільшу кількість разів");
    Console.WriteLine("8) Вивести дані про клієнтів які забронювали найбільшу кількість номерів");
    Console.WriteLine("9) Вивести дані про клієнтів та їх броні");
    Console.WriteLine("10) Вивести дані про номер, який забронювали на найдовший період");
    Console.WriteLine("11) Вивести дані про номер, у якого найбільша кількість додаткових опцій");
    Console.WriteLine("12) Вивести клас, номерів якого найбільше в готелі");
    Console.WriteLine("13) Вивести з кожного класу ті номери в який найбільша кількість місць");
    Console.WriteLine("14) Вивести клієнта який забронював більше 5 номерів");
    Console.WriteLine("15) Вивести клієнта ім'я якого починається з літери А");
    Console.WriteLine("16) Вивести всі номера та всіх клієнтів які їх бронювали");
    Console.WriteLine("17) Вивести найнепопулярніший номер(з найменшою кількістю броней)");
    Console.WriteLine("18) Вивести найдовший період броні в готелі");
    Console.WriteLine("19) Вивести номера в яких немає додаткових опцій");
    Console.WriteLine("20) Вивести клієнтів які бронювали номера без додаткових опцій");
    Console.WriteLine("21) Вивести всіх клієнтів які бронювали номера або менше чим на 10 днів або більше чим на 30 днів.");
    Console.WriteLine("22) Вивести кількість бронювань кожного номера за останній місяць:");
    Console.WriteLine("23) Вивести всі номера, відсортовані за кількістю бронювань");
    Console.WriteLine("24) Вивести всі номера, які мають опцію з певним іменем");
    Console.WriteLine("25) Вивести всі номера, відсортовані за кількістю місць");
    Console.WriteLine("26) Вивести список клієнтів, які забронювали номери більше ніж на 10 днів, відсортованих за прізвищем в алфавітному порядку");
    Console.WriteLine("27) Вивести список клієнтів, які забронювали номери заданого класу.");
    Console.WriteLine("28) Вивести список клієнтів, які забронювали номери, відсортованих за кількістю заброньованих ночей від більшого до меншого.");
    Console.WriteLine("29) Вивести всіх клієнтів, які здійснювали бронювання номерів більше 2 разів за останній місяць.");
    Console.WriteLine("30) Вивести всі номери з готелю, де не менше як половина з них має ємність більше 2-х.");
    Console.WriteLine("31) Вивести всі бронювання номерів, зроблені між 1 січня та 1 липня поточного року, де клієнт має прізвище на \"Smith\".");

    string choose = Console.ReadLine()!;
    Console.Clear();
    if (choose == "-1")
    {
        HotelManager.AllBookings();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "1")
    {
        // 1) Чи є вільний номер який відповідає бажанням клієнта
        Console.WriteLine("1)   Чи є вільний номер який відповідає бажанням клієнта");
        Console.WriteLine("Бажання до номеру у клієнта: ");
        ClientBooking prefer = new ClientBooking()
        {
            StartDate = faker.Date.Between(new DateTime(2022, 1, 1), new DateTime(2022, 6, 12)),
            EndDate = faker.Date.Between(new DateTime(2022, 6, 12), new DateTime(2023, 9, 1)),
            Capacity = 2,
            RoomClass = RoomClass.Standard,
            Options = new List<Option>() { opts[0], opts[1] }
        };
        Console.WriteLine($"Клас номеру: {prefer.RoomClass} з {prefer.Capacity} ліжками" +
            $" на період з {prefer.StartDate.ToString("yyyy-mm-dd")} до" +
            $" {prefer.EndDate.ToString("yyyy-mm-dd")} \nмісцями для сну та опціями: " +
            $"{string.Join(", ", prefer.Options.Select(x => x.Name))}");
        HotelManager.IsAnyAvailable(prefer);
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "2")
    {
        // 2) Вивести список номерів, які відповідають бажанням клієнта сортовані за номером номера
        Console.WriteLine("2)   Всі номера готелю: ");
        HotelManager.GetAllRooms();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "3")
    {
        // 3) всі заброньовані номера на даний час
        Console.WriteLine("3) всі заброньовані номера на даний час");
        HotelManager.GetBookedForNowRooms();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "4")
    {
        // 4) всі вільні номера на даний час
        Console.WriteLine("4) всі вільні номера на даний час");
        HotelManager.GetAvailableRoomsForNow();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "5")
    {
        // 5) Вивести всі номера готелю з найбільшою кількістю безкоштовних додаткових опцій
        Console.WriteLine("5) всі номера готелю з найбільшою кількістю безкоштовних додаткових опцій");
        HotelManager.GetRoomWithMostFreeAdditionalOptions();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();;
    }
    else if (choose == "6")
    {
        // 6) Вивести всі номера готелю заданого типу заброньовані на даний період
        Console.WriteLine("6) всі номера готелю заданого типу заброньовані на даний період");
        HotelManager.GetRoomsByTypeBookedForPeriod(RoomClass.Deluxe, new DateTime(2022, 12, 1), new DateTime(2023, 12, 1));
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "7")
    {
        // 7) Вивести номер номеру який бронювали найбільшу кількість разів
        Console.WriteLine("7) Вивести номер номеру який бронювали найбільшу кількість разів");
        HotelManager.GetRoomWithMostBookingTimes();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "8")
    {
        // 8) Вивести дані про клієнтів які забронювали найбільшу кількість номерів
        Console.WriteLine("8) Вивести дані про клієнтів які забронювали найбільшу кількість номерів");
        HotelManager.GetClientsWhoBookedMostCountRoom();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "9")
    {
        // 9) Вивести дані про клієнтів та їх броні
        Console.WriteLine("9) Вивести дані про клієнтів та їх броні");
        HotelManager.GetAllClientsAndThoseBookings();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "10")
    {
        // 10) Вивести дані про номер, який забронювали на найдовший період
        Console.WriteLine("10) Вивести дані про номер, який забронювали на найдовший період");
        HotelManager.GetRoomWithLongestBooking();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "11")
    {
        // 11) Вивести дані про номер, у якого найбільша кількість додаткових опцій
        Console.WriteLine("11) Вивести дані про номер, у якого найбільша кількість додаткових опцій");
        HotelManager.GetRoomWithMostCountAddOptinos();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "12")
    {
        // 12) Вивести клас, номерів якого найбільше в готелі
        Console.WriteLine("12) Вивести клас, номерів якого найбільше в готелі");
        HotelManager.GetClassWithMostCountRooms();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "13")
    {
        // 13) Вивести з кожного класу ті номери в який найбільша кількість місць
        Console.WriteLine("13) Вивести з кожного класу ті номери в який найбільша кількість місць");
        HotelManager.GetClassAndRoomsWithMostCountBeds();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "14")
    {
        // 14) Вивести клієнта який забронював більше 5 номерів
        Console.WriteLine("14) Вивести клієнта який забронював більше 5 номерів");
        HotelManager.GetClientWithBookedRoomsMoreThan5();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "15")
    {
        // 15) Вивести клієнта ім'я якого починається з літери А
        Console.WriteLine("15) Вивести клієнта ім'я якого починається з літери А");
        HotelManager.GetClientWithNameStartsFromA();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "16")
    {
        //  16) Вивести всі номера та всіх клієнтів які їх бронювали
        Console.WriteLine("16) Вивести всі номера та всіх клієнтів які їх бронювали");
        HotelManager.GetAllRoomsAndAllThoseClients();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "17")
    {
        // 17) Вивести найнепопулярніший номер(з найменшою кількістю броней)
        Console.WriteLine("17) Вивести найнепопулярніший номер(з найменшою кількістю броней)");
        HotelManager.GetMostUnpopularRoom();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "18")
    {
        // 18)  Вивести найдовший період броні в готелі
        Console.WriteLine("18) Вивести найдовший період броні в готелі");
        HotelManager.GetLongestBookPeriod();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "19")
    {
        // 19) Вивести номера в яких немає додаткових опцій
        Console.WriteLine("19) Вивести номера в яких немає додаткових опцій");
        HotelManager.GetRoomWithoutAddOpts();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "20")
    {
        // 20) Вивести клієнтів які бронювали номера без додаткових опцій
        Console.WriteLine("20) Вивести клієнтів які бронювали номера без додаткових опцій");
        HotelManager.GetClientsWhoBookedRoomsWitoutAddOpts();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "21")
    {
        // 21) Вивести всіх клієнтів які бронювали номера або менше чим на 10 днів або більше чим на 30 днів.
        Console.WriteLine("21) Вивести всіх клієнтів які бронювали номера або менше чим на 10 днів або більше чим на 30 днів.");
        HotelManager.GetClientWhoBookedLess10DOrMore30D();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose=="22")
    {
        // 22) Вивести кількість бронювань кожного номера за останній місяць:
        Console.WriteLine("22) Вивести кількість бронювань кожного номера за останній місяць:");
        HotelManager.GetCountBookLastMonth();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "23")
    {
        // 23) Вивести всі номера, відсортовані за кількістю бронювань
        Console.WriteLine("23) Вивести всі номера, відсортовані за кількістю бронювань");
        HotelManager.RoomsSortByBOokCount();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "24")
    {
        // 24) Вивести всі номера, які мають опцію з певним іменем
        Console.WriteLine("24) Вивести всі номера, які мають опцію з певним іменем");
        HotelManager.RoomsWithDefOption();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "25")
    {
        // 25) Вивести всі номера, відсортовані за кількістю місць
        Console.WriteLine("25) Вивести всі номера, відсортовані за кількістю місць");
        HotelManager.RoomsSortByCapacity();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "26")
    {
        // 26) Вивести список клієнтів, які забронювали номери більше ніж на 10 днів, відсортованих за прізвищем в алфавітному порядку
        Console.WriteLine("26) Вивести список клієнтів, які забронювали номери більше ніж на 10 днів, відсортованих за прізвищем в алфавітному порядку");
        HotelManager.ClientsBookMore10DSortSUrname();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "27")
    {
        // 27) Вивести список клієнтів, які забронювали номери заданого класу.
        Console.WriteLine("27) Вивести список клієнтів, які забронювали номери заданого класу.");
        HotelManager.ClientsBookRoomWithDefClass();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "28")
    {
        // 28) Вивести список клієнтів, які забронювали номери, відсортованих за кількістю заброньованих ночей від більшого до меншого.
        Console.WriteLine("28) Вивести список клієнтів, які забронювали номери, відсортованих за кількістю заброньованих ночей від більшого до меншого.");
        HotelManager.ClientsBookSortBookDaysDesc();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "29")
    {
        // 29) Вивести всіх клієнтів, які здійснювали бронювання номерів більше 2 разів за останній місяць.
        Console.WriteLine("29) Вивести всіх клієнтів, які здійснювали бронювання номерів більше 2 разів за останній місяць.");
        HotelManager.ClientsBookLastMon2TimsMore();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "30")
    {
        // 30) Вивести всі номери з готелю, де не менше як половина з них має ємність більше 2-х.
        Console.WriteLine("30) Вивести всі номери з готелю, де не менше як половина з них має ємність більше 2-х.");
        HotelManager.RoomsHalfCapMor2();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "31")
    {
        // 31) Вивести всі бронювання номерів, зроблені між 1 січня та 1 липня поточного року, де клієнт має прізвище на "Smith".
        Console.WriteLine("31) Вивести всі бронювання номерів, зроблені між 1 січня та 1 липня поточного року, де клієнт має прізвище на \"Smith\".");
        HotelManager.BooksBetw1Jan1JulWithClientSmith();
        Console.WriteLine("Press any key to next query");
        Console.ReadKey();
    }
    else if (choose == "exit")
    {
        Environment.Exit(0);
    }
}
