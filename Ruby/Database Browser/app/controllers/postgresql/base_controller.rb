class PostgreSQL::BaseController < ApplicationController

  helper_method :current_database

  def current_database
    @current_database ||= PostgreSQL::Database.new(name: params[:database_name] || params[:name], deployment: current_deployment)
  end

end