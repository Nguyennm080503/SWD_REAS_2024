import "./App.css";
import {
  BrowserRouter as Router,
  Route,
  Routes,
  useNavigate,
  useLocation,
  Navigate,
} from "react-router-dom";
import PageNotFound from "./Pages/PageNotFound";
import { AdminLayout } from "./Pages/Admin/AdminLayout";
import AdminDashboard from "./Pages/Admin/AdminDashboard";
import AuctionOngoing from "./Pages/Admin/AdminAuctionOngoing";
import DepositList from "./Pages/Admin/AdminCreateAuction";
import AdminStaffList from "../src/Pages/Admin/StaffList/StaffList/index";
import AdminMemberList from "../src/Pages/Admin/MemberList/MemberList/index";
import AdminAddStaff from "../src/Pages/Admin/AdminCreateStaff/AdminCreateStaff";
import PendingList from "../src/Pages/Admin/AdminRealEstatePending";
import AllList from "../src/Pages/Admin/AdminRealEstateAll"
import NewsList from "../src/Pages/Admin/AdminNews"
import AdminRule from "../src/Pages/Admin/AdminRule"
import AddRule from "../src/Pages/Admin/AdminAddRule"
import AdminCreateNews from "../src/Pages/Admin/AdminCreateNews"
import AuctionComplete from "../src/Pages/Admin/AdminAuctionComplete";
import AuctionDetail from "./Pages/Admin/AdminAuctionDetail";
import HomePage from "./Pages/Member/HomePage/homePage";
import RealEstatePage from "./Pages/Member/RealEstatePage/realEstatePage";
import HelpPage from "./Pages/Member/HelpPage/helpPage";
import MemberLayout from "./Pages/Member/memberLayout";
import AuctionPage from "./Pages/Member/AuctionPage/auctionPage";
import NewsPage from "./Pages/Member/NewsPage/newsPage";
import RequiredAuth from "./components/RequiredAuth/requiredAuth";
import SellPage from "./Pages/Member/SellPage/sellPage";
import AuctionCreate from "./Pages/Admin/AdminAuctionCreate";
import AllRule from "./Pages/Admin/AdminRule";
import CreateRule from "./Pages/Admin/AdminRuleCreate";
import AllTransaction from "./Pages/Admin/AdminTransaction";
import AllDeposit from "./Pages/Admin/AdminDeposit";
import Task from "./Pages/Admin/AdminTask";
import TaskCreate from "./Pages/Admin/AdminTaskCreate";

const roles = {
  Admin: 1,
  Staff: 2,
  Member: 3,
};

function App() {
  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path="/" element={<MemberLayout />}>
            <Route index element={<HomePage />} />
            <Route path="/realEstate" element={<RealEstatePage />} />
            <Route path="/auction" element={<AuctionPage />} />
            <Route path="/help" element={<HelpPage />} />
            <Route path="/news" element={<NewsPage />} />
            <Route element={<RequiredAuth allowedRoles={[roles.Member]} />}>
              <Route path="/sell" element={<SellPage />} />
            </Route>
          </Route>

          <Route
            element={<RequiredAuth allowedRoles={[roles.Admin, roles.Staff]} />}
          >
            <Route path="/admin" element={<AdminLayout />}>
              <Route index element={<AdminDashboard />} />

              <Route element={<RequiredAuth allowedRoles={[roles.Admin]} />}>
                <Route path="rule" element={<AllRule />} />
                <Route path="rule/create" element={<CreateRule />} />

                <Route path="task" element={<Task />} />
                <Route path="task/create" element={<TaskCreate />} />
              </Route>

              <Route path="auction/ongoing" element={<AuctionOngoing />} />
              <Route path="auction/complete" element={<AuctionComplete />} />
              <Route path="auction/create" element={<AuctionCreate />} />
              <Route path="auction/detail/:key" element={<AuctionDetail />} />

              <Route path="user/staff" element={<AdminStaffList />} />
              <Route path="user/member" element={<AdminMemberList />} />
              <Route path="user/create" element={<AdminAddStaff />} />

              <Route path="real-estate/pending" element={<PendingList />} />
              <Route path="real-estate/all" element={<AllList />} />

              <Route path="transaction" element={<AllTransaction />} />

              <Route path="deposit" element={<AllDeposit />} />
            </Route>
          </Route>

          <Route path="/unauthorized" element={<PageNotFound />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
