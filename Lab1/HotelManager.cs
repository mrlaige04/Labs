using Lab1.Models;
using Lab1.Models.Enums;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lab1
{
    public class HotelManager
    {
        public Hotel Hotel { get; set; }
        public HotelManager()
        {          
            Hotel = new Hotel();
        }


        public void AddRooms(IList<Room> rooms)
        {
            foreach (var room in rooms)
            {
                Hotel.Rooms.Add(room);
            }
        }
        public void AddBookings(IList<Booking> bookings)
        {
            foreach (var book in bookings)
            {
                Hotel.Bookings.Add(book);
                if (!Hotel.Clients.Any(x=>x.Surname==book.Client.Surname &&
                                x.Name == book.Client.Name && 
                                book.Client.Phone == x.Phone))
                    Hotel.Clients.Add(book.Client);
            }
        }
        public void AllBookings()
        {
            foreach (var book in Hotel.Bookings.OrderBy(x=>x.RoomNumber))
            {
                Console.WriteLine($"Book room # {book.RoomNumber} from {book.CheckIn.ToString("yyyy-mm-dd")} to {book.CheckOut.ToString("yyyy-mm-dd")} by {book.Client.Name} {book.Client.Surname}");
            }
        }




        
        
        // 1) Чи є вільний номер який відповідає бажанням клієнта
        public void IsAnyAvailable(ClientBooking query)
        {
            Console.WriteLine("=========================================================");
            // Extentions method           
            var queryRooms = Hotel.Rooms.Where(room =>
                room.Class == query.RoomClass &&
                room.Capacity == query.Capacity &&
                query.Options.All(opt => room.Options.Contains(opt))
            ); // All rooms that match the Client`s query            

            // Якщо взагалі такі номери не бронювались або вільні в заданий період
            var available = queryRooms.Where(room =>
                    !Hotel.Bookings.Any(book => book.RoomNumber == room.Number) ||
                    Hotel.Bookings.Where(b=>b.RoomNumber==room.Number)
                        .All(book => book.CheckOut <= query.StartDate
                )
            ).DistinctBy(x=>x.Number);

            if (available != null && available.Count() > 0)
            {
                Console.WriteLine("Є вільні номера: ");
                foreach (var avRoom in available)
                {
                    Console.WriteLine("Номер №" + avRoom.Number);
                }
            } else
            {
                Console.WriteLine("Немає вільних номерів: ");
            }
            Console.WriteLine("=========================================================");
        }

        // 2) Вивести всі номери готелю
        public void GetAllRooms()
        {
            Console.WriteLine("=========================================================");
            Hotel.Rooms.ToList().Sort(new Comparison<Room>((x, y) => 
                x.Number.CompareTo(y.Number)
            ));

            Hotel.Rooms.ToList().ForEach(x =>
            {         
                Console.WriteLine($"{x.Class} Room #{x.Number} with {x.Capacity} beds.");
                var free = x.Options.Where(x => x.Price == 0);
                var nonFree = x.Options.Where(x => x.Price > 0);

                Console.WriteLine("Free options: ");
                foreach (var freeOpt in free)
                {
                    Console.WriteLine("\t + " + freeOpt.Name);
                }

                Console.WriteLine(new string('\n', 1));
                
                Console.WriteLine("NON free options: ");
                foreach (var nonfreeOpt in nonFree)
                {
                    Console.WriteLine("\t + " + nonfreeOpt.Name);
                }
                Console.WriteLine(new string('\n',1));
            });
            Console.WriteLine("=========================================================");
        }
       
        // 3) Вивести всі заброньовані номера на даний час
        public void GetBookedForNowRooms()
        {
            Console.WriteLine("=========================================================");
            var booked = from book in Hotel.Bookings
                         where book.CheckIn <= DateTime.Now && book.CheckOut > DateTime.Now
                         select book.RoomNumber;

            foreach (var room in booked.Distinct())
            {
                Console.WriteLine("Room #"+room);
            }
            Console.WriteLine("=========================================================");
        }

        // 4) Вивести всі вільні номера на даний час
        public void GetAvailableRoomsForNow()
        {
            Console.WriteLine("=========================================================");
            var booked = from book in Hotel.Bookings
                         where book.CheckIn <= DateTime.Now && book.CheckOut > DateTime.Now
                         select book.RoomNumber;

            var allAvailable = Hotel.Rooms.Select(x=>x.Number).ToList().Except(booked);

            foreach (var item in allAvailable)
            {
                Console.WriteLine("Room #"+item);
            }
            Console.WriteLine("=========================================================");
        }
        
        // 5) Вивести всі номера готелю з найбільшою кількістю безкоштовних додаткових опцій
        public void GetRoomWithMostFreeAdditionalOptions()
        {
            Console.WriteLine("=========================================================");
            var maxFreeOptions = Hotel.Rooms
                    .GroupBy(x => x.Options.Count(x => x.Price == 0))
                    .OrderByDescending(x => x.Key)
                    .First();         

            if (maxFreeOptions != null && maxFreeOptions.Count() > 0)
            {
                foreach (var free in maxFreeOptions)
                {
                    Console.WriteLine("Room #" + free.Number + $" with {free.Options.Count} free options");
                }           
            }
            Console.WriteLine("=========================================================");
        }
        
        // 6) Вивести всі номера готелю заданого типу заброньовані на даний період
        public void GetRoomsByTypeBookedForPeriod(RoomClass @class, DateTime @from, DateTime to)
        {
            Console.WriteLine("=========================================================");
            var booksInPeriod = from b in Hotel.Bookings
                                where b.CheckIn == @from && b.CheckOut == to
                                select b.RoomNumber;

            var classRooms = from r in Hotel.Rooms
                             from b in booksInPeriod
                             where b == r.Number && r.Class == @class
                             select r;

            foreach (var gr in classRooms)
            {
                Console.WriteLine("\t Room number: " + gr.Number);
            }
            
            Console.WriteLine("=========================================================");
        }
        
        // 7) Вивести номер номеру який бронювали найбільшу кількість разів
        public void GetRoomWithMostBookingTimes()
        {
            Console.WriteLine("=========================================================");
            var mostBookedRoomNumber = Hotel.Bookings
                    .GroupBy(b => b.RoomNumber)
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;

            Console.WriteLine("Room # " + mostBookedRoomNumber);

            Console.WriteLine("=========================================================");
        }

        // 8) Вивести дані про клієнтів які забронювали найбільшу кількість номерів
        public void GetClientsWhoBookedMostCountRoom()
        {
            Console.WriteLine("=========================================================");
            var maxBookingCount = Hotel.Bookings
                .GroupBy(booking => booking.Client) // групування бронювань по клієнту
                .Select(group => new { Client = group.Key, Count = group.Count() }) // вибір клієнту та кількості його бронювань
                .Max(group => group.Count); // знаходження максимальної кількості бронювань серед усіх клієнтів

            var topBookingClients = Hotel.Bookings
                .GroupBy(booking => booking.Client) // групування бронювань по клієнту
                .Where(group => group.Count() == maxBookingCount) // фільтрація клієнтів з максимальною кількістю бронювань
                .Select(group => group.Key); // вибір клієнтів з максимальною кількістю бронювань

            Console.WriteLine("max times: " + maxBookingCount);
            foreach (var client in topBookingClients)
            {
                Console.WriteLine($"{client.Surname} {client.Name}");
            }

            Console.WriteLine("=========================================================");
        }
        
        // 9) Вивести дані про клієнтів та їх броні
        public void GetAllClientsAndThoseBookings()
        {
            Console.WriteLine("=========================================================");
            var bookingsWithClients = from booking in Hotel.Bookings
                                      join client in Hotel.Clients
                                      on new { booking.Client.Surname, booking.Client.Name, booking.Client.Phone } equals new { client.Surname, client.Name, client.Phone }
                                      select new
                                      {
                                          ClientName = client.Name + " " + client.Surname,
                                          RoomNumber = booking.RoomNumber,
                                          CheckIn = booking.CheckIn,
                                          CheckOut = booking.CheckOut
                                      };
            foreach (var booking in bookingsWithClients.OrderBy(x=>x.ClientName))
            {
                Console.WriteLine($"Client: {booking.ClientName}, Room: {booking.RoomNumber}, Check-in: {booking.CheckIn}, Check-out: {booking.CheckOut}");
            }
            Console.WriteLine("=========================================================");
        }

        // 10) Вивести дані про номер, який забронювали на найдовший період
        public void GetRoomWithLongestBooking()
        {
            Console.WriteLine("=========================================================");
            var longestBooking = Hotel.Bookings
                    .OrderByDescending(b => (b.CheckOut - b.CheckIn).TotalDays)
                    .FirstOrDefault();

            if (longestBooking != null)
            {
                var roomNumber = longestBooking.RoomNumber;
                Console.WriteLine($"Номер з найбільшою тривалістю бронювання: {roomNumber}");
                Console.WriteLine($"В період з {longestBooking.CheckIn} до {longestBooking.CheckOut}");
                Console.WriteLine((longestBooking.CheckOut - longestBooking.CheckIn).TotalDays + " днів.");
            }
            else
            {
                Console.WriteLine("Немає бронювань");
            }
            Console.WriteLine("=========================================================");
        }

        // 11) Вивести дані про номер, у якого найбільша кількість додаткових опцій
        public void GetRoomWithMostCountAddOptinos()
        {
            Console.WriteLine("=========================================================");
            var mostOptions = (from r in Hotel.Rooms
                               orderby r.Options.Count descending
                               select r).First();
            if (mostOptions != null) Console.WriteLine("Room #"+mostOptions.Number);
            Console.WriteLine("=========================================================");
        }

        // 12) Вивести клас, номерів якого найбільше в готелі
        public void GetClassWithMostCountRooms()
        {
            Console.WriteLine("=========================================================");
            var roomsByClass = from r in Hotel.Rooms
                               group r by r.Class into g
                               select new { Class = g.Key, Count = g.Count() };

            var maxClass = roomsByClass.OrderByDescending(r => r.Count).First();

            Console.WriteLine($"The class with the most rooms is {maxClass.Class} with {maxClass.Count} rooms.");
            Console.WriteLine("=========================================================");
        }

        // 13) Вивести з кожного класу ті номери в який найбільша кількість місць
        public void GetClassAndRoomsWithMostCountBeds()
        {
            Console.WriteLine("=========================================================");
            var maxCapacityByClass = from room in Hotel.Rooms
                                     group room by room.Class into roomGroup
                                     let maxCapacity = roomGroup.Max(room => room.Capacity)
                                     select new
                                     {
                                         RoomClass = roomGroup.Key,
                                         RoomNumbers = roomGroup.Where(room => room.Capacity == maxCapacity)
                                                              .Select(room => room.Number)
                                     };
            foreach (var item in maxCapacityByClass)
            {
                Console.WriteLine("Room class: " +  item.RoomClass);
                foreach (var room in item.RoomNumbers)
                {
                    Console.WriteLine("\t Room number: " + room);
                }
            }
            Console.WriteLine("=========================================================");
        }

        // 14) Вивести клієнта який забронював більше 5 номерів
        public void GetClientWithBookedRoomsMoreThan5()
        {
            Console.WriteLine("=========================================================");
            var clientsWithBookings = from booking in Hotel.Bookings
                                      group booking by booking.Client into clientBookings
                                      select new
                                      {
                                          Client = clientBookings.Key,
                                          BookingsCount = clientBookings.Count()
                                      };

            var clientsWithFivePlusBookings = clientsWithBookings
                                              .Where(cb => cb.BookingsCount > 5)
                                              .Select(cb => cb.Client);

            foreach (var client in clientsWithFivePlusBookings)
            {
                Console.WriteLine($"Client: {client.Name} {client.Surname}");
            }
            Console.WriteLine("=========================================================");
        }

        // 15) Вивести клієнта ім'я якого починається з літери А
        public void GetClientWithNameStartsFromA()
        {
            Console.WriteLine("=========================================================");
            var clientsWithA = Hotel.Bookings
                .Where(booking => booking.Client.Name.StartsWith("A"))
                .Select(booking => booking.Client);

            foreach (var client in clientsWithA)
            {
                Console.WriteLine(client.Name);
            }
            Console.WriteLine("=========================================================");
        }

        // 16) Вивести всі номера та всіх клієнтів які їх бронювали
        public void GetAllRoomsAndAllThoseClients()
        {
            Console.WriteLine("=========================================================");
            var query = Hotel.Rooms.Join(Hotel.Bookings, 
                room => room.Number, booking => booking.RoomNumber, 
                (room, booking) => new { RoomNumber = room.Number, ClientName = booking.Client.Name +" "+ booking.Client.Surname });

            foreach (var item in query)
            {
                Console.WriteLine($"Room number: {item.RoomNumber}, Client name: {item.ClientName}");
            }
            Console.WriteLine("=========================================================");
        }

        // 17) Вивести найнепопулярніший номер(з найменшою кількістю броней)
        public void GetMostUnpopularRoom()
        {
            Console.WriteLine("=========================================================");
            var leastPopularRoom = Hotel.Bookings
                    .GroupBy(booking => booking.RoomNumber)
                    .OrderBy(group => group.Count())
                    .FirstOrDefault();

            if (leastPopularRoom != null)
            {
                Console.WriteLine($"The least popular room is number {leastPopularRoom.Key}");
            }
            
            Console.WriteLine("=========================================================");
        }

        // 18)  Вивести найдовший період броні в готелі
        public void GetLongestBookPeriod()
        {
            Console.WriteLine("=========================================================");        
            var longestBooking = Hotel.Bookings.Max(booking => (booking.CheckOut - booking.CheckIn).Days);
            Console.WriteLine($"Longest booking period: {longestBooking} days");
            Console.WriteLine("=========================================================");
        }

        // 19) Вивести номера в яких немає додаткових опцій
        public void GetRoomWithoutAddOpts()
        {
            Console.WriteLine("=========================================================");
            var roomsWithoutOptions = from room in Hotel.Rooms
                                      where room.Options == null || room.Options.Count == 0
                                      select room.Number;
            foreach (var room in roomsWithoutOptions)
            {
                Console.WriteLine("Room #"+room);
            }
            Console.WriteLine("=========================================================");
        }

        // 20) Вивести клієнтів які бронювали номера без додаткових опцій
        public void GetClientsWhoBookedRoomsWitoutAddOpts()
        {
            Console.WriteLine("=========================================================");
            var query = from room in Hotel.Rooms
                        join booking in Hotel.Bookings
                        on room.Number equals booking.RoomNumber
                        where room.Options == null || room.Options.Count == 0
                        select booking.Client;

            foreach (var client in query.Distinct())
            {
                Console.WriteLine($"Client name: {client.Name}");
            }
            Console.WriteLine("=========================================================");
        }

        // 21) Вивести всіх клієнтів які бронювали номера або менше чим на 10 днів або більше чим на 30 днів.
        public void GetClientWhoBookedLess10DOrMore30D()
        {
            Console.WriteLine("=========================================================");
            var clients = Hotel.Bookings
                    .Where(b => (b.CheckOut - b.CheckIn).Days <= 10 || (b.CheckOut - b.CheckIn).Days >= 30) // фільтр за тривалістю бронювання
                    .Select(b => b.Client) // отримання всіх клієнтів, які брали участь в бронюванні
                    .Distinct(); // видалення дублікатів клієнтів

            foreach (var client in clients)
            {
                Console.WriteLine($"Client name: {client.Name + " "+ client.Surname}");
            }
            Console.WriteLine("=========================================================");
        }

        //22) Вивести кількість бронювань кожного номера за останній місяць:
        public void GetCountBookLastMonth()
        {
            Console.WriteLine("=========================================================");
            var lastMonthBookings = Hotel.Bookings.Where(b => b.CheckIn >= DateTime.Now.AddMonths(-1))
                                 .GroupBy(b => b.RoomNumber)
                                 .Select(g => new { RoomNumber = g.Key, Count = g.Count() });

            foreach (var item in lastMonthBookings)
            {
                Console.WriteLine($"Room number: {item.RoomNumber}, Bookings count: {item.Count}");
            }
            Console.WriteLine("=========================================================");
        }
        
        //23) Вивести всі номера, відсортовані за кількістю бронювань
        public void RoomsSortByBOokCount()
        {
            Console.WriteLine("=========================================================");
            var sortedRooms = Hotel.Rooms.OrderBy(r => Hotel.Bookings.Count(b => b.RoomNumber == r.Number));

            foreach (var room in sortedRooms)
            {
                Console.WriteLine($"Room number: {room.Number}, Bookings count: {Hotel.Bookings.Count(b => b.RoomNumber == room.Number)}");
            }
            Console.WriteLine("=========================================================");
        }
        
        //24) Вивести всі номера, які мають опцію з певним іменем
        public void RoomsWithDefOption()
        {
            Console.WriteLine("=========================================================");
            var filteredRooms = Hotel.Rooms.Where(r => r.Options.Any(o => o.Name == "Wi-Fi"));

            foreach (var room in filteredRooms)
            {
                Console.WriteLine($"Room number: {room.Number}");
            }
            Console.WriteLine("=========================================================");
        }
        
        //25) Вивести всі номера, відсортовані за кількістю місць
        public void RoomsSortByCapacity()
        {
            Console.WriteLine("=========================================================");
            var sortedRooms = Hotel.Rooms.OrderBy(r => r.Capacity);

            foreach (var room in sortedRooms)
            {
                Console.WriteLine($"Room number: {room.Number}, Capacity: {room.Capacity}");
            }
            Console.WriteLine("=========================================================");
        }
        
        //26) Вивести список клієнтів, які забронювали номери більше ніж на 10 днів, відсортованих за прізвищем в алфавітному порядку
        public void ClientsBookMore10DSortSUrname()
        {
            Console.WriteLine("=========================================================");
            var query = from booking in Hotel.Bookings
                        where (booking.CheckOut - booking.CheckIn).TotalDays > 10
                        orderby booking.Client.Surname
                        select booking.Client;

            foreach (var client in query)
            {
                Console.WriteLine($"{client.Name} {client.Surname}");
            }
            Console.WriteLine("=========================================================");
        }
        
        //27) Вивести список клієнтів, які забронювали номери заданого класу.
        public void ClientsBookRoomWithDefClass()
        {
            Console.WriteLine("=========================================================");
            var roomClass = RoomClass.Apartment;

            var query = from booking in Hotel.Bookings
                        from r in Hotel.Rooms
                        where booking.RoomNumber == r.Number && r.Class == roomClass
                        select booking.Client;

            foreach (var client in query)
            {
                Console.WriteLine($"{client.Name} {client.Surname}");
            }
            Console.WriteLine("=========================================================");
        }
        
        //28) Вивести список клієнтів, які забронювали номери, відсортованих за кількістю заброньованих ночей від більшого до меншого.
        public void ClientsBookSortBookDaysDesc()
        {
            Console.WriteLine("=========================================================");
            var query = from booking in Hotel.Bookings
                        group booking by booking.Client into clientBookings
                        let totalNights = clientBookings.Sum(booking => (booking.CheckOut - booking.CheckIn).TotalDays)
                        orderby totalNights descending
                        select clientBookings.Key;

            foreach (var client in query)
            {
                Console.WriteLine($"{client.Name} {client.Surname}");
            }
            Console.WriteLine("=========================================================");
        }
        
        //29) Вивести всіх клієнтів, які здійснювали бронювання номерів більше 2 разів за останній місяць.
        public void ClientsBookLastMon2TimsMore()
        {
            Console.WriteLine("=========================================================");
            var lastMonth = DateTime.Now.AddMonths(-1);
            var result = Hotel.Clients
                .Where(c => c.Bookings.Count(b => b.CheckIn >= lastMonth) > 2)
                .Select(c => new { c.Name, c.Surname, c.Phone });

            foreach (var item in result)
            {
                Console.WriteLine(item.Name + " " + item.Surname + " " +item.Phone);
            }
            Console.WriteLine("=========================================================");
        }
        
        //30) Вивести всі номери з готелю, де не менше як половина з них має ємність більше 2-х.
        public void RoomsHalfCapMor2()
        {
            Console.WriteLine("=========================================================");
            var result = Hotel.Rooms
                .GroupBy(r => r.Class)
                .Where(g => g.Count(r => r.Capacity > 2) >= g.Count() / 2)
                .SelectMany(g => g)
                .Select(r => r.Number);
            foreach (var item in result)
            {
                Console.WriteLine("room #"+item);
            }
            Console.WriteLine("=========================================================");
        }
        
        //31) Вивести всі бронювання номерів, зроблені між 1 січня та 1 липня поточного року, де клієнт має прізвище на "Smith".
        public void BooksBetw1Jan1JulWithClientSmith()
        {
            Console.WriteLine("=========================================================");
            var result = Hotel.Bookings
                .Where(b => b.CheckIn >= new DateTime(DateTime.Now.Year, 1, 1) && b.CheckIn <= new DateTime(DateTime.Now.Year, 7, 1))
                .Where(b => b.Client.Surname == "Smith")
                .Select(b => new { b.CheckIn, b.CheckOut, ClientName = $"{b.Client.Name} {b.Client.Surname}" });
            Console.WriteLine("=========================================================");
        }
    }
}
