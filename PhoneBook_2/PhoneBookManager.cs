using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook_2
{
    
    public class PhoneBookManager
    {
        //수정시 수정부분 다 주석하고 오류부분들을 다 고친다
        //const int MAX_CNT = 100; 
        //PhoneInfo[] infoStorage = new PhoneInfo[MAX_CNT];
        //private int curCnt = 0;

        HashSet<PhoneInfo> infoStorage = new HashSet<PhoneInfo>(); // 해쉬셋 생성
        static PhoneBookManager inst = null;
        readonly string dataFile = "PhoneBook.dat"; //바이너리, bin debug 안에 생성된다

        private PhoneBookManager() // 기본생성자 만들고 private 붙여서 new 사용 못하게 함, 싱글톤
        {
            ReadToFile();
        }
        private void ReadToFile()
        {
            if (!File.Exists(dataFile))
                return;
            try
            {
                FileStream fs = new FileStream(dataFile, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();

                infoStorage.Clear();

                infoStorage = (HashSet<PhoneInfo>)formatter.Deserialize(fs);
                fs.Close();
            }
            catch(IOException err)
            {
                Console.WriteLine(err.Message);
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
        public void WriteToFile()
        {
            try
            {
                
                using (FileStream fs = new FileStream(dataFile, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, infoStorage);
                }
            }
            catch(Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
       
        public static PhoneBookManager CreateMangerInstance() //반환타입이 PhoneBookManager인 메서드 생성, static 을 붙여서 인스턴스 생성 없이 부름
        {
            if (inst == null)
                inst = new PhoneBookManager(); //inst가 널이면 새로운 PhoneBookManager()생성
            return inst; //이미 생성되었다면 원래있던 inst 전달
        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowMenu()
        {
            Console.WriteLine("------------------------ 주소록 -----------------------------------");
            Console.WriteLine("1. 입력  |  2. 목록  |  3. 검색  |  4. 정렬  |  5. 삭제  |  6. 종료");
            Console.WriteLine("------------------------------------------------------------------");
            Console.Write("선택: ");
        }
        /// <summary>
        /// 
        /// </summary>
        public void SortData()
        {            
            int choice;
            while (true)
            {
                try
                {
                    Console.WriteLine("1.이름 ASC  2.이름 DESC  3.전화번호 ASC  4.전화번호 DESC");
                    Console.Write("선택 >> ");

                    if (int.TryParse(Console.ReadLine(), out choice))
                    {
                        if (choice < 1 || choice > 4)
                        {
                            throw new MenuChoiceException(choice);

                            //Console.WriteLine("1.이름 ASC  2.이름 DESC  3.전화번호 ASC  4.전화번호 DESC 중에 선택하십시오.");
                            //return;
                        }
                        else
                            break;
                    }
                }
                catch(MenuChoiceException err)
                {
                    err.ShowWrongChoice();
                }
            }

            //PhoneInfo[] new_arr = new PhoneInfo[curCnt];
            //Array.Copy(infoStorage, new_arr, curCnt);

            List<PhoneInfo> list = new List<PhoneInfo>(infoStorage);

            if (choice == 1)
            {
                list.Sort(new NameComparator());
                //Array.Sort(new_arr, new NameComparator());
            }
            else if (choice == 2)
            {
                list.Sort(new NameComparator());
                list.Reverse();
                //Array.Sort(new_arr, new NameComparator());
                //Array.Reverse(new_arr);
            }
            else if (choice == 3)
            {
                list.Sort(new PhoneComparator());
                //Array.Sort(new_arr, new PhoneComparator());
            }
            else if (choice == 4)
            {
                list.Sort(new PhoneComparator());
                list.Reverse();
            }

            foreach(PhoneInfo curInfo in list)
            {
                curInfo.ShowPhoneInfo();
                Console.WriteLine();
            }
            //for (int i = 0; i < curCnt; i++)
            //{
            //    Console.WriteLine(new_arr[i].ToString());
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        public void InputData()
        {
            Console.WriteLine("1.일반  2.대학  3.회사");
            Console.Write("선택 >> ");
            int choice;
            while (true)
            {                
                if (int.TryParse(Console.ReadLine(), out choice))
                    break;
            }
            if (choice < 1 || choice > 3)
            {
                Console.WriteLine("1.일반  2.대학  3.회사 중에 선택하십시오.");
                return;
            }

            PhoneInfo info = null;
            switch(choice)
            {
                case 1:
                    info = InputFriendInfo();
                    break;
                case 2:
                    info = InputUnivInfo(); 
                    break;
                case 3:
                    info = InputCompanyInfo(); 
                    break;
            }
            if (info != null)
            {
                //infoStorage[curCnt++] = info;
                bool isAdded = infoStorage.Add(info); //해쉬셋 특 들어왔는지 안왔는지 bool타입으로 반환함
                if(isAdded)
                    Console.WriteLine("데이터 입력이 완료되었습니다");
                else
                    Console.WriteLine("이미 저장된 데이터입니다");
            }
        }

        private List<string> InputCommonInfo()
        {
            Console.Write("이름: ");
            string name = Console.ReadLine().Trim();
            //if (name == "") or if (name.Length < 1) or if (name.Equals(""))
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("이름은 필수입력입니다");
                return null;
            }
            else
            {
                if (SearchName(name))
                {
                    Console.WriteLine("이미 등록된 이름입니다. 다른 이름으로 입력하세요");
                    return null;
                }
            }

            Console.Write("전화번호: ");
            string phone = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(phone))
            {
                Console.WriteLine("전화번호는 필수입력입니다");
                return null;
            }

            Console.Write("생일: ");
            string birth = Console.ReadLine().Trim();

            List<string> list = new List<string>();
            list.Add(name);
            list.Add(phone);
            list.Add(birth);

            return list;
        }

        private PhoneInfo InputFriendInfo()
        {
            List<string> comInfo = InputCommonInfo();
            if (comInfo == null || comInfo.Count != 3)
                return null;

            return new PhoneInfo(comInfo[0], comInfo[1], comInfo[2]);
        }

        private PhoneInfo InputUnivInfo()
        {
            List<string> comInfo = InputCommonInfo();
            if (comInfo == null || comInfo.Count != 3)
                return null;

            Console.Write("전공: ");
            string major = Console.ReadLine().Trim();

            Console.Write("학년: ");
            int year = int.Parse(Console.ReadLine().Trim());

            return new PhoneUnivInfo(comInfo[0], comInfo[1], comInfo[2], major, year);
        }

        private PhoneInfo InputCompanyInfo()
        {
            List<string> comInfo = InputCommonInfo();
            if (comInfo == null || comInfo.Count != 3)
                return null;

            Console.Write("회사명: ");
            string company = Console.ReadLine().Trim();

            return new PhoneCompanyInfo(comInfo[0], comInfo[1], comInfo[2], company);
        }
        /// <summary>
        /// 
        /// </summary>
        public void ListData()
        {
            if (infoStorage.Count == 0)
            {
                Console.WriteLine("입력된 데이터가 없습니다.");
                return;
            }
            foreach(PhoneInfo curInfo in infoStorage)
            {
                curInfo.ShowPhoneInfo();
                Console.WriteLine();
            }
            //for(int i=0; i<curCnt; i++)
            //{
            //    //infoStorage[i].ShowPhoneInfo();
            //    //Console.WriteLine();

            //    Console.WriteLine(infoStorage[i].ToString());                
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        public void SearchData()
        {
            Console.WriteLine("주소록 검색을 시작합니다......");
            PhoneInfo findInfo = SearchName();
            if(findInfo == null)
            {
                Console.WriteLine("검색된 데이터가 없습니다");
            }
            else
            {
                findInfo.ShowPhoneInfo();
                Console.WriteLine();
            }
            //int dataIdx = SearchName();
            //if (dataIdx < 0)
            //{
            //    Console.WriteLine("검색된 데이터가 없습니다");
            //}
            //else
            //{
            //    infoStorage[dataIdx].ShowPhoneInfo();
            //    Console.WriteLine();
            //}

            #region 모두 찾기
            //int findCnt = 0;
            //for(int i=0; i<curCnt; i++)
            //{
            //    // ==, Equals(), CompareTo()
            //    if (infoStorage[i].Name.Replace(" ","").CompareTo(name) == 0)
            //    {
            //        infoStorage[i].ShowPhoneInfo();
            //        findCnt++;
            //    }
            //}
            //if (findCnt < 1)
            //{
            //    Console.WriteLine("검색된 데이터가 없습니다");
            //}
            //else
            //{
            //    Console.WriteLine($"총 {findCnt} 명이 검색되었습니다.");
            //}
            #endregion
        }

        //private int SearchName()
        //{
        //    Console.Write("이름: ");
        //    string name = Console.ReadLine().Trim().Replace(" ", "");

        //    for (int i = 0; i < curCnt; i++)
        //    {
        //        if (infoStorage[i].Name.Replace(" ", "").CompareTo(name) == 0)
        //        {
        //            return i;
        //        }
        //    }

        //    return -1;
        //}
        
        private PhoneInfo SearchName()
        {
            
            Console.Write("이름: ");
            string name = Console.ReadLine().Trim().Replace(" ", "");

           foreach(PhoneInfo curInfo in infoStorage)
            {
                if (name.CompareTo(curInfo.Name) == 0)
                {
                    return curInfo;                    
                }
            }
            return null;
            
        }
        
        private bool SearchName(string name)
        {
            foreach(PhoneInfo curInfo in infoStorage)
            {
                if (curInfo.Name.Equals(name))
                {
                    return true;
                }
                
            }
            return false;
            //for (int i = 0; i < curCnt; i++)
            //{
            //    if (infoStorage[i].Name.Replace(" ", "").CompareTo(name) == 0)
            //    {
            //        return i;
            //    }
            //}

            //return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        public void DeleteData()
        {
            Console.WriteLine("주소록 삭제를 시작합니다......");

            PhoneInfo delInfo = SearchName();
            if (delInfo == null)
            {
                Console.WriteLine("삭제할 데이터가 없습니다");
            }
            else
            {
                infoStorage.Remove(delInfo);
                //for(int i=dataIdx; i<curCnt; i++)
                //{
                //    infoStorage[i] = infoStorage[i + 1];
                //}
                //curCnt--;
                Console.WriteLine("주소록 삭제가 완료되었습니다");
            }
        }
    }
}
