namespace DragonNutrex.App.Interfaces;

/// <summary>
/// Define el contrato para las operaciones de acceso a datos en el repositorio genérico.
/// Proporciona métodos estándar para crear, actualizar, eliminar y recuperar entidades del tipo especificado.
/// </summary>
/// <typeparam name="T">Tipo de entidad con la que opera el repositorio.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Crea una nueva entidad en el repositorio.
    /// Almacena la entidad proporcionada en la fuente de datos.
    /// </summary>
    /// <param name="entity">Entidad que se va a crear.</param>
    void Create(T entity);

    /// <summary>
    /// Actualiza una entidad existente en el repositorio.
    /// Modifica los datos de la entidad especificada en la fuente de datos.
    /// </summary>
    /// <param name="entity">Entidad con los datos actualizados.</param>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad del repositorio basándose en su identificador único.
    /// Remueve la entidad correspondiente de la fuente de datos.
    /// </summary>
    /// <param name="id">Identificador único (GUID) de la entidad a eliminar.</param>
    void Delete(Guid id);

    /// <summary>
    /// Obtiene todas las entidades del repositorio de forma síncrona.
    /// Recupera la totalidad de registros almacenados en la fuente de datos.
    /// </summary>
    /// <returns>Lista conteniendo todas las entidades disponibles del tipo especificado.</returns>
    List<T> GetAll();

    /// <summary>
    /// Obtiene todas las entidades del repositorio de forma asíncrona.
    /// Recupera la totalidad de registros almacenados en la fuente de datos sin bloquear la ejecución.
    /// </summary>
    /// <returns>Tarea que devuelve una lista conteniendo todas las entidades disponibles del tipo especificado.</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad específica del repositorio utilizando su identificador único.
    /// Recupera un registro individual basándose en el GUID proporcionado.
    /// </summary>
    /// <param name="id">Identificador único (GUID) de la entidad a recuperar.</param>
    /// <returns>Entidad encontrada del tipo especificado, o null si no existe.</returns>
    T? GetById(Guid id);
}