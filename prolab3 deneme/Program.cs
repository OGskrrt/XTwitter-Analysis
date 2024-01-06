using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;

class LinkedList<A> : IEnumerable<A>
{
    private class Node
    {
        public A Data { get; }
        public Node Next { get; set; }

        public Node(A data)
        {
            Data = data;
            Next = null;
        }
    }

    private Node head;

    public void Add(A data)
    {
        Node newNode = new Node(data);

        if (head == null)
        {
            head = newNode;
        }
        else
        {
            Node temp = head;

            while (temp.Next != null)
            {
                temp = temp.Next;
            }

            temp.Next = newNode;
        }
    }

    public int IndexOf(A data)
    {
        Node temp = head;
        int index = 0;

        while (temp != null)
        {
            if (Equals(temp.Data, data))
            {
                return index;
            }

            temp = temp.Next;
            index++;
        }

        return -1;
    }

    public A Get(int index)
    {
        Node temp = head;

        for (int i = 0; i < index; i++)
        {
            temp = temp.Next;
        }

        return temp.Data;
    }

    public bool Contains(A data)
    {
        Node temp = head;

        while (temp != null)
        {
            if (Equals(temp.Data, data))
            {
                return true;
            }

            temp = temp.Next;
        }

        return false;
    }

    public void AddToEnd(A data)
    {
        Node newNode = new Node(data);

        if (head == null)
        {
            head = newNode;
        }
        else
        {
            Node current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
    }

    public void RemoveFirst()
    {
        if (head != null)
        {
            head = head.Next;
        }
    }

    public IEnumerator<A> GetEnumerator()
    {
        Node temp = head;
        while (temp != null)
        {
            yield return temp.Data;
            temp = temp.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

class User
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string FollowersCount { get; set; }
    public string FollowingCount { get; set; }
    public string Language { get; set; }
    public string Region { get; set; }
    public List<string> Tweets { get; set; } = new List<string>();
    public List<string> Following { get; set; } = new List<string>();
    public List<string> Followers { get; set; } = new List<string>();
    public string Interest { get; set; }

    public void DisplayInfo()
    {
        Console.WriteLine($"Username: {Username}");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Followers Count: {FollowersCount}");
        Console.WriteLine($"Following Count: {FollowingCount}");
        Console.WriteLine($"Language: {Language}");
        Console.WriteLine($"Region: {Region}");
        Console.WriteLine("Tweets:");
        foreach (var tweet in Tweets)
        {
            Console.WriteLine($"  - {tweet}");
        }
        Console.WriteLine("Following:");
        foreach (var followingUser in Following)
        {
            Console.WriteLine($"  - {followingUser}");
        }
        Console.WriteLine("Followers:");
        foreach (var followerUser in Followers)
        {
            Console.WriteLine($"  - {followerUser}");
        }
        Console.WriteLine("\n");
    }
}

class Graph
{
    public static Dictionary<string, User> users = new Dictionary<string, User>();
    public string[] kullanicidizi = new string[users.Count];

    public struct Edge
    {
        public int Sink; // diğer uç
    }

    public struct Successors
    {
        public int D; // bağlı halef sayısı, alt düğüm
        public int Len; // boş slot sayısı
        public Edge[] List; // kenar sayısı
    }

    public struct GraphStructure
    {
        public int N; // düğüm sayısı
        public int M; // kenar sayısı
        public Successors[] AList; // kaynak düğümler
    }

    private void kullanicidiziupdate()
    {
        kullanicidizi = new string[users.Count]; 

        int i = 0;
        foreach (var user in users.Values)
        {
            kullanicidizi[i++] = user.Username;
        }
    }

    GraphStructure graphmibu;
    public void biribirineeklegraph()
    {
        kullanicidiziupdate();


        graphmibu.N = users.Count;
        graphmibu.M = 0;
        graphmibu.AList = new Successors[graphmibu.N]; //kullanıcı sayısı kadar successor oluştur (halef)

        int i = 0;
        foreach (var user in users.Values)
        {
            Console.WriteLine($"graphadd yükleniyor ... {i}");
            graphmibu.AList[i] = new Successors();
            graphmibu.AList[i].D = user.Following.Count;
            graphmibu.AList[i].Len = user.Following.Count;
            graphmibu.AList[i].List = new Edge[user.Following.Count]; //kullanıcı takipçi sayısı kadar edge oluştur

            int j = 0;
            foreach (var friendUsername in user.Following)
            {
                int hedef = Array.IndexOf(kullanicidizi, friendUsername);
                graphmibu.AList[i].List[j].Sink = hedef;
                j++;
            }

            graphmibu.M += user.Following.Count;
            i++;
        }
    }
    public void graphyazdir()
    {
        for (int i = 0; i < graphmibu.N; i++)
        {
            Console.WriteLine($"Kullanıcı: {kullanicidizi[i]}");

            Console.WriteLine("Takip Ettikleri:");
            for (int j = 0; j < graphmibu.AList[i].D; j++)
            {
                int hedefIndex = graphmibu.AList[i].List[j].Sink;
                string hedefUsername = kullanicidizi[hedefIndex];
                Console.Write($"  - {hedefUsername}");
            }

            Console.WriteLine();
        }
    }
    public void AddUser(User user)
    {
        if (!users.ContainsKey(user.Username))
        {
            users.Add(user.Username, user);
        }
    }
    public void BFS(string kullanici)
    {
        int basla = Array.IndexOf(kullanicidizi, kullanici);

        if (basla == -1)
        {
            Console.WriteLine($"Kullanıcı '{kullanici}' bulunamadı.");
            return;
        }

        bool[] gecildi = new bool[graphmibu.N];
        Queue<int> queue = new Queue<int>();

        gecildi[basla] = true;
        queue.Enqueue(basla);

        Console.WriteLine("BFS algoritması:");

        while (queue.Count > 0)
        {
            basla = queue.Dequeue();

            Console.Write($"{kullanicidizi[basla]} ");

            foreach (Edge edge in graphmibu.AList[basla].List)
            {
                int yetisinkomsular = edge.Sink;

                if (!gecildi[yetisinkomsular])
                {
                    gecildi[yetisinkomsular] = true;
                    queue.Enqueue(yetisinkomsular);

                    
                    int takipciSayisi1 = graphmibu.AList[basla].List.Count();
                    int takipciSayisi2 = graphmibu.AList[yetisinkomsular].List.Count();

                    
                    if (Math.Abs(takipciSayisi1 - takipciSayisi2) <= 10)
                    {
                        Console.Write($"{kullanicidizi[yetisinkomsular]} - ");
                    }
                }
            }
        }

        Console.WriteLine();
    }



    /*
    public void PrintGraph()
    {
        foreach (var user in users.Values)
        {
            Console.Write($"{user.Name}'s friends: ");
            foreach (var friendUsername in user.Following)
            {
                Console.Write($"{friendUsername} ");

            }
            Console.WriteLine();
            Console.WriteLine("takipçiler");
            Console.WriteLine();
            foreach (var friendUsername in user.Followers)
            {
                Console.Write($"{friendUsername} ");

            }
            Console.WriteLine();
        }
    }*/
    
    /*                                                                  dijsktra orjinal hali c kodu
     * void
    dijkstra(Graph g, int source, int *dist, int *parent)
    {
    struct push_data data;
    struct pq_elt e;
    int n;
    int i;

    assert(dist);

    data.dist = dist;
    data.pq = pq_create(sizeof(struct pq_elt), pq_elt_cmp);
    assert(data.pq);

    n = graph_vertex_count(g);

    /* set up dist and parent arrays * /
    for(i = 0; i < n; i++) {
        dist[i] = MAXINT;
    }
        
    if(parent) {
        for(i = 0; i < n; i++) {
            parent[i] = DIJKSTRA_NULL_PARENT;
        }
    }

    /* push (source, source, 0) * /
    /* this will get things started with parent[source] == source * /
    /* and dist[source] == 0 * /
    push(g, source, source, -MAXINT, &data);

    while(!pq_is_empty(data.pq)) {
        /* pull the min value out * /
        pq_delete_min(data.pq, &e);

        /* did we reach the sink already? * /
        if(dist[e.v] == MAXINT) {
            /* no, it's a new edge * /
            dist[e.v] = e.d;
            if(parent) parent[e.v] = e.u;

            /* throw in the outgoing edges * /
            graph_foreach_weighted(g, e.v, push, &data);
        }
    }

    pq_destroy(data.pq);
    }
     */
    public int Dijkstra(string kaynak, string hedef)
    {
        kullanicidiziupdate();
        
        int kaynakindex = Array.IndexOf(kullanicidizi, kaynak);
        int hedefindex = Array.IndexOf(kullanicidizi, hedef);

        if (kaynakindex == -1 || hedefindex == -1)
        {
            Console.WriteLine("Böyle bir kaynak veya hedef yok");
            return 0;
        }

        int n = graphmibu.N;

        int[] mesafe = new int[n];  // distance
        int[] birust = new int[n];   //parent dizisi
        bool[] gecildi = new bool[n];      //visited
        

        for (int i = 0; i < n; i++)  //dolduruver
        {
            mesafe[i] = int.MaxValue;
            birust[i] = -1;
            gecildi[i] = false;
        }

        mesafe[kaynakindex] = 0;
        // dijsktra başlasin
        
        for (int count = 0; count < n - 1; count++)
        {
            Console.WriteLine($"Dijkstra yükleniyor {count}");
            int u = enkisayol(mesafe, gecildi);
            gecildi[u] = true;

            for (int v = 0; v < n; v++)
            {
                if (!gecildi[v] && graphmibu.AList[u].List.Any(edge => edge.Sink == v))
                {
                    int edgeWeight = 1; 

                    if (mesafe[u] + edgeWeight < mesafe[v])
                    {
                        mesafe[v] = mesafe[u] + edgeWeight;
                        birust[v] = u;
                    }
                }
            }
        }

        EnkisayolDijkstra(kaynakindex, hedefindex, birust);
        return 1;
    }

    private int enkisayol(int[] mesafe, bool[] gecildi)
    {
        int min = int.MaxValue, minIndex = -1;

        for (int v = 0; v < mesafe.Length; v++)
        {
            if (!gecildi[v] && mesafe[v] <= min)
            {
                min = mesafe[v];
                minIndex = v;
            }
        }

        return minIndex;
    }

    private void EnkisayolDijkstra(int source, int target, int[] birust)
    {
        List<string> yol = new List<string>();
        int curr = target;

        while (curr != -1)
        {
            yol.Add(kullanicidizi[curr]);
            curr = birust[curr];
        }

        yol.Reverse();

        Console.WriteLine($"İki kullanıcı arasında en kısa yol: {kullanicidizi[source]} -> {kullanicidizi[target]}: {yol.Count - 1}");
        Console.WriteLine("Yol: " + string.Join(" -> ", yol));  //tek tek yazdrıma şeysi
    }
    public void WriteGraphToFile(string filePath)
    {
        Console.Clear();
        Console.WriteLine("Veriler dosyaya yazılıyor...");

        try
        {
            // StreamWriter'ı kullanarak dosyayı oluşturun veya var olan bir dosyayı açın (true parametresi dosyanın üzerine yazılmasını sağlar)
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (var user in users.Values)
                {
                    sw.WriteLine($"Username: {user.Username}");
                    sw.WriteLine($"Name: {user.Name}");
                    sw.WriteLine($"Followers Count: {user.Followers.Count()}");
                    sw.WriteLine($"Following Count: {user.Following.Count()}");
                    sw.WriteLine($"Language: {user.Language}");
                    sw.WriteLine($"Region: {user.Region}");
                    sw.Write("Following:");
                    foreach (var followingUser in user.Following)
                    {
                        sw.Write($" {followingUser}");
                    }
                    sw.WriteLine();
                    sw.Write("Followers:");
                    foreach (var followerUser in user.Followers)
                    {
                        sw.Write($" {followerUser}");
                    }
                    sw.WriteLine();
                    sw.Write($"Interest: {user.Interest}");
                    sw.WriteLine("\n");
                }
                Console.Clear();
                Console.WriteLine("Dosya yazma işlemi tamamlandı. \n\nDosya yolu: " + filePath + "\n");
                Thread.Sleep(5000);
            }
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine($"Programı yönetici olacak çalıştırın: {ex.Message}");
            Thread.Sleep(5000);
        }
    }
    LinkedList<string>[] interestList ;
    
    public void Interests2()
    {
        string[] filterWords = { "ve", "ama", "bir", "bu", "şu", "o", "ben", "sen", "o", "biz", "siz", "onlar",
                                 "ve", "ile", "ise", "de", "da", "ki", "ne", "neden", "nasıl", "hangi", "bunu", "buna",
                                 "şunu", "şuna", "ona", "sana", "bana", "beni", "seni", "onu", "biz", "sizi", "onları",
                                 "kendi", "kendine", "kendini", "diğer", "öteki", "öbür", "her", "hep", "bazı", "birkaç",
                                 "tüm", "hiçbir", "şey", "şeyi", "herkes", "kimse", "bir şey", "şeyler", "gibi", "olarak",
                                 "buna göre", "buna bağlı", "göre", "gibi", "ise", "diye", "ancak", "ama", "fakat", "lakin",
                                 "ise de", "ama", "fakat", "ki", "çünkü", "çünkü ki", "zira", "madem", "göre", "göre ki",
                                 "rağmen", "rağmen ki", "hâlbuki", "hâlbuki ki", "oysa", "oysaki", "ve", "ile", "ya da",
                                 "veya", "ya da", "veya ki", "hem", "hem de", "sadece", "yalnız", "yani", "yani ki",
                                 "mesela", "örneğin", "örnek", "eğer", "eğer ki", "madem", "madem ki", "hatta", "üstelik",
                                 "üstelik ki", "bilâkis", "bilâkis ki", "aksine", "aksine ki", "oysa ki", "o halde",
                                 "o halde ki", "öyleyse", "öyleyse ki", "sonuç olarak", "sonuçta", "dolayısıyla",
                                 "dolayısıyla ki", "demek", "demek ki", "tamam", "tabii", "elbette", "kuşkusuz",
                                 "kesinlikle", "daha", "için", "kesin", "çünkü", "bile", "iki", "üç", "dört", "beş",
                                 "altı", "yedi", "sekiz", "dokuz", "on", "şekilde", "şeklinde", "ilk", "çok", "sonra",
                                 "kadar", "tarafından", "ayrıca", "en", "sonuçta", "olarak", "var", "etti", "aynı",
                                 "oldu", "sonrasında", "şundan", "bundan", "şöyle", "böyle", "dolayı", "birçok", "zamanda",
                                 "olmuştur", "olmuş", "az", "olmuştu", "vardır", "yer", "olan", "artık", "alır", "arasında",
                                 "yapmıştır", "almıştır", "olan", "bulunmaktadır", "mezunudur", "göre", "böylece", "aldı",
                                 "kaldı", "büyük", "küçük", "özellikle", "genellikle", "olur", "olduğunu", "birinci", "ikinci",
                                 "üçüncü", "dördüncü", "beşinci", "gelen", "boyunca", "başladı", "yapılır", "yaptı", "olduğu",
                                 "pek", "yap", "yapar", "yapmış", "yapıyor", "yapıl", "yapılmış", "yapılacak", "al", "alır",
                                 "almış", "alıyor", "alın", "alınmış", "alınacak", "ver", "verir", "vermiş", "veriyor",
                                 "veril", "verilmiş", "verilecek", "git", "gider", "gitmiş", "gidiyor", "gidil", "gidilmiş",
                                 "gidilecek", "eder", "etmiş", "ediyor", "edil", "edilmiş", "edilecek", "oku", "okur",
                                 "okumuş", "okuyor", "okun", "okunmuş", "okunacak", "yazar", "yazmış", "yazıyor", "yazıl",
                                 "yazılmış", "yazılacak", "gör", "görür", "görmüş", "görüyor", "görül", "görülmüş",
                                 "görülecek", "iyi", "kötü", "büyük", "küçük", "güzel", "çirkin", "hızlı", "yavaş", "eski",
                                 "yeni", "genç", "yaşlı", "uzun", "kısa", "sert", "yumuşak", "sıcak", "soğuk", "yumuşak",
                                 "sert", "hafif", "ağır", "zor", "kolay", "güçlü", "zayıf", "sağlıklı", "hasta", "mutlu",
                                 "tanınmış", "birliği", "farklı", "yazmıştır", "nedenle", "olduğu", "nehri", "üniversitesi",
                                 "geri", "içinde", "genel", "genelde", "içindedir", "çeşitli", "devam", "hemen", "belki"};

        kullanicidiziupdate(); // Kullanıcı dizisini güncelle

        foreach (var user in users.Values)
        {
            // Tüm tweet kelimelerini saklamak için bir dizi oluştur
            List<string> tumkelime = new List<string>();

            foreach (var tweet in user.Tweets)
            {
                string[] words = tweet.Split(' ', '.', ',', ';', ':', '?', '!', '\t', '\n', '\r');

                foreach (var word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        string temiz = word.ToLower();

                        // Filtreleme
                        if (!filterWords.Contains(temiz))
                        {
                            tumkelime.Add(temiz);
                        }
                    }
                }
            }

            string[] tümkelimeleryeni = tumkelime.ToArray();

            // Her kelimenin frekansını hesaplamak için bir dizi oluştur
            string[] tümkelimelerdahayeni = tümkelimeleryeni.Distinct().ToArray();  //disticnt ??
            int[] freq = new int[tümkelimelerdahayeni.Length];

            // Her kelimenin frekansını hesapla
            for (int i = 0; i < tümkelimelerdahayeni.Length; i++)
            {
                for (int j = 0; j < tümkelimeleryeni.Length; j++)
                {
                    if (tümkelimelerdahayeni[i] == tümkelimeleryeni[j])
                    {
                        freq[i]++;
                    }
                }
            }

            // En çok geçen kelimeyi bul
            int maxfreq = Array.IndexOf(freq, freq.Max());

            if (maxfreq != -1)
            {
                // Kullanıcı dizisi indeksine göre ilgiyi ayarla
                int kullindex = Array.IndexOf(kullanicidizi, user.Username);
                if (kullindex != -1)
                {
                    users[kullanicidizi[kullindex]].Interest = tümkelimelerdahayeni[maxfreq];
                }
            }
        }
    }
    /*public void Interests()
    {
        Dictionary<string, Dictionary<string, int>> userInterests = new Dictionary<string, Dictionary<string, int>>();
        Dictionary<string, KeyValuePair<string, int>> userMaxInterest = new Dictionary<string, KeyValuePair<string, int>>();

        // Filtrelenmesi istenen kelimeler
        string[] filterWords = { "ve", "ama", "bir", "bu", "şu", "o", "ben", "sen", "o", "biz", "siz", "onlar",
                                 "ve", "ile", "ise", "de", "da", "ki", "ne", "neden", "nasıl", "hangi", "bunu", "buna",
                                 "şunu", "şuna", "ona", "sana", "bana", "beni", "seni", "onu", "biz", "sizi", "onları",
                                 "kendi", "kendine", "kendini", "diğer", "öteki", "öbür", "her", "hep", "bazı", "birkaç",
                                 "tüm", "hiçbir", "şey", "şeyi", "herkes", "kimse", "bir şey", "şeyler", "gibi", "olarak",
                                 "buna göre", "buna bağlı", "göre", "gibi", "ise", "diye", "ancak", "ama", "fakat", "lakin",
                                 "ise de", "ama", "fakat", "ki", "çünkü", "çünkü ki", "zira", "madem", "göre", "göre ki",
                                 "rağmen", "rağmen ki", "hâlbuki", "hâlbuki ki", "oysa", "oysaki", "ve", "ile", "ya da",
                                 "veya", "ya da", "veya ki", "hem", "hem de", "sadece", "yalnız", "yani", "yani ki",
                                 "mesela", "örneğin", "örnek", "eğer", "eğer ki", "madem", "madem ki", "hatta", "üstelik",
                                 "üstelik ki", "bilâkis", "bilâkis ki", "aksine", "aksine ki", "oysa ki", "o halde",
                                 "o halde ki", "öyleyse", "öyleyse ki", "sonuç olarak", "sonuçta", "dolayısıyla",
                                 "dolayısıyla ki", "demek", "demek ki", "tamam", "tabii", "elbette", "kuşkusuz",
                                 "kesinlikle", "daha", "için", "kesin", "çünkü", "bile", "iki", "üç", "dört", "beş",
                                 "altı", "yedi", "sekiz", "dokuz", "on", "şekilde", "şeklinde", "ilk", "çok", "sonra",
                                 "kadar", "tarafından", "ayrıca", "en", "sonuçta", "olarak", "var", "etti", "aynı",
                                 "oldu", "sonrasında", "şundan", "bundan", "şöyle", "böyle", "dolayı", "birçok", "zamanda",
                                 "olmuştur", "olmuş", "az", "olmuştu", "vardır", "yer", "olan", "artık", "alır", "arasında",
                                 "yapmıştır", "almıştır", "olan", "bulunmaktadır", "mezunudur", "göre", "böylece", "aldı",
                                 "kaldı", "büyük", "küçük", "özellikle", "genellikle", "olur", "olduğunu", "birinci", "ikinci",
                                 "üçüncü", "dördüncü", "beşinci", "gelen", "boyunca", "başladı", "yapılır", "yaptı", "olduğu",
                                 "pek", "yap", "yapar", "yapmış", "yapıyor", "yapıl", "yapılmış", "yapılacak", "al", "alır",
                                 "almış", "alıyor", "alın", "alınmış", "alınacak", "ver", "verir", "vermiş", "veriyor",
                                 "veril", "verilmiş", "verilecek", "git", "gider", "gitmiş", "gidiyor", "gidil", "gidilmiş",
                                 "gidilecek", "eder", "etmiş", "ediyor", "edil", "edilmiş", "edilecek", "oku", "okur",
                                 "okumuş", "okuyor", "okun", "okunmuş", "okunacak", "yazar", "yazmış", "yazıyor", "yazıl",
                                 "yazılmış", "yazılacak", "gör", "görür", "görmüş", "görüyor", "görül", "görülmüş",
                                 "görülecek", "iyi", "kötü", "büyük", "küçük", "güzel", "çirkin", "hızlı", "yavaş", "eski",
                                 "yeni", "genç", "yaşlı", "uzun", "kısa", "sert", "yumuşak", "sıcak", "soğuk", "yumuşak",
                                 "sert", "hafif", "ağır", "zor", "kolay", "güçlü", "zayıf", "sağlıklı", "hasta", "mutlu",
                                 "tanınmış", "birliği", "farklı", "yazmıştır", "nedenle", "olduğu", "nehri", "üniversitesi",
                                 "geri", "içinde", "genel", "genelde", "içindedir", "çeşitli", "devam", "hemen", "belki"};

        foreach (var user in users.Values)
        {
            Dictionary<string, int> tweetWordFrequency = new Dictionary<string, int>();

            foreach (var tweet in user.Tweets)
            {
                string[] words = tweet.Split(' ', '.', ',', ';', ':', '?', '!', '\t', '\n', '\r');

                foreach (var word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        string cleanedWord = word.ToLower();

                        // Filtreleme
                        if (!filterWords.Contains(cleanedWord))
                        {
                            if (tweetWordFrequency.ContainsKey(cleanedWord))
                            {
                                tweetWordFrequency[cleanedWord]++;
                            }
                            else
                            {
                                tweetWordFrequency[cleanedWord] = 1;
                            }
                        }
                    }
                }
            }

            var maxInterest = tweetWordFrequency.OrderByDescending(kv => kv.Value).FirstOrDefault();
            if (maxInterest.Key != null)
            {
                user.Interest = $"{maxInterest.Key}";
                
            }
        }
    }*/

    private List<LinkedList<string>> ilgiListesi = new List<LinkedList<string>>();

    public Hashtable IlgiAlaniTablosuOlustur()
    {
        Hashtable tablo = new Hashtable();

        foreach (var user in users.Values)
        {
            if (!string.IsNullOrEmpty(user.Interest))
                tablo.Add(user.Interest, user.Username);
        }

        return tablo;
    }

    public void ListeyeEkle(int indis, string username)
    {
        LinkedList<string> liste = ilgiListesi[indis];

        if(liste == null)
        {
            liste = new LinkedList<string>();
            ilgiListesi[indis] = liste;
        }

        liste.Add(username);
    }
    Dictionary<string, List<string>> ilgiGruplar = new Dictionary<string, List<string>>();
    public void ListeyeYerlestir(Hashtable ilgiTablo)
    {
        

        foreach (var user in users.Values)
        {
            if (!string.IsNullOrEmpty(user.Interest))
            {
                string ilgiDeger = ilgiTablo.Get(user.Interest);

                if (ilgiDeger != null)
                {
                    // Anahtar zaten mevcutsa, listeyi al; değilse yeni bir liste oluştur
                    List<string> kullaniciListesi;
                    if (!ilgiGruplar.TryGetValue(user.Interest, out kullaniciListesi))
                    {
                        kullaniciListesi = new List<string>();
                        ilgiGruplar[user.Interest] = kullaniciListesi;
                    }

                    kullaniciListesi.Add(user.Username);
                }
            }
        }

        using (StreamWriter sw = new StreamWriter(@"C:\\verbanailgi.txt"))
        {
            foreach (var ilgiGrup in ilgiGruplar)
            {
                sw.WriteLine($"ilgilenilen konu: {ilgiGrup.Key}, kişiler: {string.Join(", ", ilgiGrup.Value)}");
            }
        }

        Console.WriteLine("Veriler C:\\verbanailgi.txt dosyasına yazıldı.");
    }
    
    public void ilgialaniara(string ilgialani)
    {
        

        foreach (var ilgiGrup in ilgiGruplar)
        {
            if (ilgialani == ilgiGrup.Key.ToString())
            {
                Console.WriteLine($"ilgilenilen konu: {ilgiGrup.Key}, kişiler: {string.Join(", ", ilgiGrup.Value)}");
                
            }
        }
    }


    private Dictionary<string, List<string>> interestMap;

    public Graph()
    {
        interestMap = new Dictionary<string, List<string>>();
    }

    /*public void AddUser(string user, string interest)
    {
        if (!interestMap.ContainsKey(interest))
        {
            interestMap[interest] = new List<string>();
        }

        interestMap[interest].Add(user);
    }*/
    public void bolgeilgi(string hedefBolge)
    {
        string[] ilgiAlanlari = new string[users.Count];  // klasik freakans hesaplama
        int[] ilgiAlanSayilari = new int[users.Count]; 

        int index = 0;

        
        foreach (var kullanici in users.Values)
        {
            
            if (kullanici.Region == hedefBolge && !string.IsNullOrEmpty(kullanici.Interest))
            {
                
                int ilgiIndex = Array.IndexOf(ilgiAlanlari, kullanici.Interest);
                if (ilgiIndex != -1)
                {
                    ilgiAlanSayilari[ilgiIndex]++;
                }
                else
                {
                    ilgiAlanlari[index] = kullanici.Interest;
                    ilgiAlanSayilari[index]++;
                    index++;
                }
            }
        }

        
        if (index > 0)
        {
            
            string enCokCikanIlgilenilenKonu = ilgiAlanlari[0];
            int enCokCikanIlgilenilenKonuAdet = ilgiAlanSayilari[0];

            for (int i = 1; i < index; i++)
            {
                if (ilgiAlanSayilari[i] > enCokCikanIlgilenilenKonuAdet)
                {
                    enCokCikanIlgilenilenKonu = ilgiAlanlari[i];
                    enCokCikanIlgilenilenKonuAdet = ilgiAlanSayilari[i];
                }
            }

            
            Console.WriteLine($"Hedef Bölge: {hedefBolge}");
            Console.WriteLine($"Bölgede ilgilenilen konu: {enCokCikanIlgilenilenKonu} ({enCokCikanIlgilenilenKonuAdet} kez)");
        }
        else
        {
            Console.WriteLine($"Hedef Bölge: {hedefBolge}");
            Console.WriteLine("Herhangi bir ilgi alanı bulunamadı.");
        }
    }


}


class Hashtable
{
    private int GeciciKapasite = 50000;
    private float DolulukOrani = 0.75f;

    private List<List<KeyValuePair<string, string>>> hucreler;
    private int count = 0;

    public Hashtable()
    {
        hucreler = new List<List<KeyValuePair<string, string>>>(GeciciKapasite);

        for (int i = 0; i < GeciciKapasite; i++)
            hucreler.Add(null);
    }

    private void BoyutArttir()
    {
        if((float)count / hucreler.Count > DolulukOrani)
        {
            int yeniKapasite = hucreler.Count * 2;
            var yeniHucreler = new List<List<KeyValuePair<string, string>>>(yeniKapasite);

            foreach(var hucre in hucreler)
            {
                if (hucre != null)
                    yeniHucreler.Add(new List<KeyValuePair<string, string>>(hucre));

                else
                    yeniHucreler.Add(null);
            }

            hucreler = yeniHucreler;
        }
    }

    private int GetHucreIndis(string key, int kapasite)
    {
        return Math.Abs(key.GetHashCode() % kapasite);
    }

    public void Add(string key, string deger)
    {
        BoyutArttir();

        int indis = GetHucreIndis(key, hucreler.Count);

        if (hucreler[indis] == null)
            hucreler[indis] = new List<KeyValuePair<string, string>>();

        var hucre = hucreler[indis];

        for(int i = 0; i < hucre.Count; i++)
        {
            var es = hucre[i];
            if(es.Key == key)
            {
                hucre[i] = new KeyValuePair<string, string>(key, deger);
                return;
            }
        }

        hucre.Add(new KeyValuePair<string, string>(key, deger));
        count++;
    }

    public string Get(string key)
    {
        int indis = GetHucreIndis(key, hucreler.Count);
        var hucre = hucreler[indis];

        if(hucre != null)
        {
            foreach(var es in hucre)
            {
                if (es.Key == key)
                    return es.Value;
            }
        }

        return null;
    }
}


class Program
{
    static List<User> LoadDataFromJson(string filePath)
    {
        List<User> data = null;
        try
        {
            string json = File.ReadAllText(filePath);
            data = JsonConvert.DeserializeObject<List<User>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading JSON from {filePath}: {ex.Message}");
        }
        return data;
    }

    static Dictionary<string, User> CreateUserObjects(List<User> userData, Graph graph)
    {
        Dictionary<string, User> users = new Dictionary<string, User>();
        foreach (var user in userData)
        {
            graph.AddUser(user);
            users[user.Username] = user;
        }
        foreach (var user in userData)
        {
            foreach (var followerUsername in user.Followers)
            {
                if (users.ContainsKey(followerUsername))
                {
                    User followerUser = users[followerUsername];
                    //graph.AddFriendship(followerUser, user);
                }
            }
        }
        return users;
    }


    static void Main()
    {
        string jsonFilePath = @"C:\Users\skrrrt\twitter_data_50K.json";
        List<User> data = LoadDataFromJson(jsonFilePath);

        if (data != null)
        {
            Graph graph = new Graph();
            Dictionary<string, User> users = CreateUserObjects(data, graph);
            string dosyaYolu = "C:\\dosya.txt";

            
            graph.Interests2();
            graph.WriteGraphToFile(dosyaYolu);
            Hashtable interestTable = graph.IlgiAlaniTablosuOlustur();
            graph.ListeyeYerlestir(interestTable);
            
            //Thread.Sleep(100000);
            Console.WriteLine($"Main graph ekleme öncesi");
            graph.biribirineeklegraph();

            bool menu = true;
            while (menu)
            {
                bool devam = true;
                while (devam)
                {
                    Console.WriteLine("Dijkstra için kaynak kişiyi giriniz: ");
                    string sourceUsername = Console.ReadLine();
                    Console.WriteLine("Hedef kişiyi giriniz: ");
                    string targetUsername = Console.ReadLine();
                    graph.Dijkstra(sourceUsername, targetUsername);
    
                    Console.WriteLine("Dijkstra'dan çıkmak için 0 giriniz: ");
                    string devamInput = Console.ReadLine();
                    devam = (devamInput != "0");
                }

                devam = true;
                while (devam)
                {
                    // girilien kişinin alt ağacında takipçi saysı +- 10 olan kişileri yazar
                    Console.Write("Görmek istediğiniz ağacın kaynak kişisini giriniz: ");
                    string bfsuser = Console.ReadLine();
                    graph.BFS(bfsuser);
                    Console.WriteLine("BFS algoritmasından çıkmak için 0 giriniz: ");
                    string devamInput = Console.ReadLine();
                    devam = (devamInput != "0");
                
                }
            
                devam = true;
                while (devam)
                {
                    // girilien kişinin alt ağacında takipçi saysı +- 10 olan kişileri yazar
                    Console.Write("Aramak istediğiniz ilgi alanini giriniz: ");
                    string ilgi = Console.ReadLine();
                    graph.ilgialaniara(ilgi);
                    Console.WriteLine("İlgi alani aramadan çıkmak için 0 giriniz: ");
                    string devamInput = Console.ReadLine();
                    devam = (devamInput != "0");
                
                }
            
                devam = true;
                while (devam)
                {
                
                    Console.Write("Ayni ilgi alanina sahip kişilerin arasındaki bağlantıyı bulmak için ilk kişiyi giriniz: ");
                    string sourceUsername = Console.ReadLine();
                    Console.WriteLine("Hedef kişiyi giriniz: ");
                    string targetUsername = Console.ReadLine();
                    graph.Dijkstra(sourceUsername, targetUsername);
                
                    Console.WriteLine("İlgi alani içindeki aramadan çıkmak için 0 giriniz: ");
                    string devamInput = Console.ReadLine();
                    devam = (devamInput != "0");
                
                }
            
                devam = true;
                while (devam)
                {
                
                    Console.Write("Hangi bölgedeki ilgi alanını bulmak istiyorsunuz?: ");
                    string blgeilgi = Console.ReadLine();
                    graph.bolgeilgi(blgeilgi);
               
                
                    Console.WriteLine("bölge ilgi alani arama içinden aramadan çıkmak için 0 giriniz: ");
                    string devamInput = Console.ReadLine();
                    devam = (devamInput != "0");
                
                }
                Console.WriteLine("Menuden çıkmak için 0 giriniz, tekrar için herhangi bir tuşa basınız: ");
                string menuInput = Console.ReadLine();
                menu = (menuInput != "0");
            }
            
            
            
            
        }
        else
        {
            Console.WriteLine("Failed to load JSON data. Exiting...");
        }
    }
}