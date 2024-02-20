import React from "react";

const SearchBar = () => {
  return (
    <div className="w-full relative">
      <img
        src="../../public/Search-bar-bg.jpg"
        alt=""
        className="w-full h-96 object-cover"
      />
      <div className="absolute inset-0 flex items-center justify-center">
        <form className="lg:max-w-lg sm:max-w-md mx-auto w-full">
          <div className="text-center text-4xl mb-4 text-white font-bold">
            Find. Auction. Deposit. Own.
          </div>
          <label
            htmlFor="default-search"
            className="mb-2 text-sm font-medium text-gray-900 sr-only dark:text-white"
          >
            Search
          </label>
          <div className="relative">
            <div className="absolute inset-y-0 start-0 flex items-center ps-3 pointer-events-none">
              <svg
                className="w-4 h-4 text-gray-500 dark:text-gray-400"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 20 20"
              >
                <path
                  stroke="currentColor"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="m19 19-4-4m0-7A7 7 0 1 1 1 8a7 7 0 0 1 14 0Z"
                />
              </svg>
            </div>
            <input
              type="search"
              id="default-search"
              className="block sm:w-full p-4 ps-10 text-sm text-gray-900 border border-gray-300 rounded-3xl bg-gray-50 focus:ring-mainBlue focus:border-mainBlue focus:outline-none"
              placeholder="Search for real estates, auctions, news, or addresses,..."
            />
          </div>
        </form>
      </div>
    </div>
  );
};

export default SearchBar;
